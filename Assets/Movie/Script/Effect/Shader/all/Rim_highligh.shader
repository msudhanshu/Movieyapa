// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Rim_Highlight" {
   Properties {
      _Color ("Color", Color) = (1, 0, 1, 1) 
      _RimColor("RimColor",Color) = (1,0,0,1)
      _alpha ("Alpha", Float) = 0.7
      _thickness ("Boundary Thickness", Float) = 0.3
      _MainTex ("Base Texture", 2D) = "white" {} 
         // user-specified RGBA color including opacity
   }
   SubShader {
     // Tags { "Queue" = "Transparent" } 
         // draw after all opaque geometry has been drawn
      Pass { 
       //  ZWrite Off // don't occlude other objects
      //   Blend SrcAlpha OneMinusSrcAlpha // standard alpha blending
 
         CGPROGRAM 
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
 			
 		 uniform sampler2D _MainTex;
 		 uniform float4 _MainTex_ST;
         uniform float4 _Color; // define shader property for shaders
         uniform float4 _RimColor;
 		 uniform float _thickness;
 		 uniform float _alpha;
 		 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texCoord : TEXCOORD0;
         };
         
         struct vertexOutput {
            float4 pos : SV_POSITION;
			float2 texCoord : TEXCOORD0;
           //  float3 normal : TEXCOORD0;
           // float3 viewDir : TEXCOORD1;
            float newOpacity;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
 
            float3 normal = normalize(float3(
               mul(float4(input.normal, 0.0), modelMatrixInverse)));
            float3 viewDir = normalize(_WorldSpaceCameraPos 
               - float3(mul(modelMatrix, input.vertex)));
			output.newOpacity = min( 1- dot(normalize(viewDir), normalize(normal) ), 1);
            //  	output.newOpacity = min(1.0, _alpha / pow( abs(dot(normalize(viewDir), normalize(normal) )), _thickness) );
                
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            output.texCoord = TRANSFORM_TEX (input.texCoord, _MainTex);
           
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
       	  if(input.newOpacity < _thickness)
           	 return float4( tex2D(_MainTex, input.texCoord).xyz , 1);
            else
             return _RimColor + float4( tex2D(_MainTex, input.texCoord).xyz , 1);
         }
 
         ENDCG
      }
   }
}