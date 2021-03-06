﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "EnvironmentMap_Refraction" {
   Properties {
      _Cube("Reflection Map", Cube) = "" {}
   }
   SubShader {
      Pass {   
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
 
         // User-specified uniforms
         uniform samplerCUBE _Cube;   
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float3 normalDir : TEXCOORD0;
            float3 viewDir : TEXCOORD1;
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
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float refractiveIndex = 1.5;
            float3 refractedDir = refract(normalize(input.viewDir), 
               normalize(input.normalDir), 1.0 / refractiveIndex);
            return texCUBE(_Cube, refractedDir);
         }
 
         ENDCG
      }
   }
}