// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Silhouette" {
   Properties {
      _Color ("Color", Color) = (1, 0, 1, 0.5) 
      _thickness ("Boundary Thickness", Float) = 0.5
         // user-specified RGBA color including opacity
   }
   SubShader {
      Tags { "Queue" = "Transparent" } 
         // draw after all opaque geometry has been drawn
      Pass { 
         ZWrite Off // don't occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha // standard alpha blending
 
         CGPROGRAM 
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
 
         uniform float4 _Color; // define shader property for shaders
 		 uniform float _thickness;
 		 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float3 normal : TEXCOORD;
            float3 viewDir : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
 
            output.normal = normalize(float3(
               mul(float4(input.normal, 0.0), modelMatrixInverse)));
            output.viewDir = normalize(_WorldSpaceCameraPos 
               - float3(mul(modelMatrix, input.vertex)));
 
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normal);
            float3 viewDirection = normalize(input.viewDir);
 
            float newOpacity = min(1.0, _Color.a 
               / pow( abs(dot(viewDirection, normalDirection)), _thickness) );
            return float4(float3(_Color), newOpacity);
         }
 
         ENDCG
      }
   }
}