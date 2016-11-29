// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "EnvironmentMap_Textured" {
   Properties {
      _Cube("Reflection Map", Cube) = "" {}
      _MainTex("MainTexture", 2D) = "white" {}
      _lerpValue ("Linear Interpolation Value", Range(0.0 ,1.0)) = 0.5
   }
   SubShader {
      Pass {   
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
 
         // User-specified uniforms
         uniform samplerCUBE _Cube;   
 		 uniform sampler2D _MainTex;
 		 uniform float4 _MainTex_ST;
 		 uniform float _lerpValue;
 		 
         struct vertexInput {
            float4 vertex : POSITION;
            float2  texcoord : TEXCOORD0;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float2  uv : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
            float3 viewDir : TEXCOORD2;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
 
            output.viewDir = float3(mul(modelMatrix, input.vertex) 
               - float4(_WorldSpaceCameraPos, 1.0));
            output.normalDir = normalize(float3(
               mul(float4(input.normal, 0.0), modelMatrixInverse)));
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            
            output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	 half4 texcol = tex2D (_MainTex, input.uv);
            float3 reflectedDir = 
               reflect(input.viewDir, normalize(input.normalDir));
            half4 texcubecol =  texCUBE(_Cube, reflectedDir);
            
            return lerp(texcol, texcubecol, _lerpValue);
            //return texcol + texcubecol;
         }
 
         ENDCG
      }
   }
}