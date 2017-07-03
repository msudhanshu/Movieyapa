﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "TranslucentSurfaces" {
 //Shader "Cg translucent surfaces" {
   Properties {
      _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
      _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Float) = 10
      _DiffuseTranslucentColor ("Diffuse Translucent Color", Color) 
         = (1,1,1,1) 
      _ForwardTranslucentColor ("Forward Translucent Color", Color) 
         = (1,1,1,1) 
      _Sharpness ("Sharpness", Float) = 10
   }
   SubShader {
      Pass {      
         Tags { "LightMode" = "ForwardBase" } 
            // pass for ambient light and first light source
         Cull Off // show frontfaces and backfaces
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform float4 _Color; 
         uniform float4 _SpecColor; 
         uniform float _Shininess;
         uniform float4 _DiffuseTranslucentColor; 
         uniform float4 _ForwardTranslucentColor; 
         uniform float _Sharpness;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
 
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(float3(
               mul(float4(input.normal, 0.0), modelMatrixInverse)));
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normalDir);
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - float3(input.posWorld));
 
            normalDirection = faceforward(normalDirection,
               -viewDirection, normalDirection);
               // flip normal if dot(-viewDirection, normalDirection)>0
 
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = 
                  normalize(float3(_WorldSpaceLightPos0));
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  float3(_WorldSpaceLightPos0 - input.posWorld);
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            // Computation of the Phong reflection model:
 
            float3 ambientLighting = 
               float3(UNITY_LIGHTMODEL_AMBIENT) * float3(_Color);
 
            float3 diffuseReflection = 
               attenuation * float3(_LightColor0) * float3(_Color)
               * max(0.0, dot(normalDirection, lightDirection));
 
            float3 specularReflection;
            if (dot(normalDirection, lightDirection) < 0.0) 
               // light source on the wrong side?
            {
               specularReflection = float3(0.0, 0.0, 0.0); 
                  // no specular reflection
            }
            else // light source on the right side
            {
               specularReflection = attenuation * float3(_LightColor0) 
                  * float3(_SpecColor) * pow(max(0.0, dot(
                  reflect(-lightDirection, normalDirection), 
                  viewDirection)), _Shininess);
            }
 
            // Computation of the translucent illumination:
 
            float3 diffuseTranslucency = 
               attenuation * float3(_LightColor0) 
               * float3(_DiffuseTranslucentColor) 
               * max(0.0, dot(lightDirection, -normalDirection));
 
            float3 forwardTranslucency;
            if (dot(normalDirection, lightDirection) > 0.0) 
               // light source on the wrong side?
            {
               forwardTranslucency = float3(0.0, 0.0, 0.0); 
                  // no forward-scattered translucency
            }
            else // light source on the right side
            {
               forwardTranslucency = attenuation * float3(_LightColor0)
                  * float3(_ForwardTranslucentColor) * pow(max(0.0, 
                  dot(-lightDirection, viewDirection)), _Sharpness);
            }
 
            // Computation of the complete illumination:
 
            return float4(ambientLighting 
               + diffuseReflection + specularReflection 
               + diffuseTranslucency + forwardTranslucency, 1.0);
         }
 
         ENDCG
      }
 
      Pass {      
         Tags { "LightMode" = "ForwardAdd" } 
            // pass for additional light sources
         Cull Off
         Blend One One // additive blending 
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform float4 _Color; 
         uniform float4 _SpecColor; 
         uniform float _Shininess;
         uniform float4 _DiffuseTranslucentColor; 
         uniform float4 _ForwardTranslucentColor; 
         uniform float _Sharpness;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
 
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(float3(
               mul(float4(input.normal, 0.0), modelMatrixInverse)));
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normalDir);
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - float3(input.posWorld));
 
            normalDirection = faceforward(normalDirection,
               -viewDirection, normalDirection);
               // flip normal if dot(-viewDirection, normalDirection)>0
 
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = 
                  normalize(float3(_WorldSpaceLightPos0));
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  float3(_WorldSpaceLightPos0 - input.posWorld);
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            // Computation of the Phong reflection model:
 
            float3 ambientLighting = 
               float3(UNITY_LIGHTMODEL_AMBIENT) * float3(_Color);
 
            float3 diffuseReflection = 
               attenuation * float3(_LightColor0) * float3(_Color)
               * max(0.0, dot(normalDirection, lightDirection));
 
            float3 specularReflection;
            if (dot(normalDirection, lightDirection) < 0.0) 
               // light source on the wrong side?
            {
               specularReflection = float3(0.0, 0.0, 0.0); 
                  // no specular reflection
            }
            else // light source on the right side
            {
               specularReflection = attenuation * float3(_LightColor0) 
                  * float3(_SpecColor) * pow(max(0.0, dot(
                  reflect(-lightDirection, normalDirection), 
                  viewDirection)), _Shininess);
            }
 
            // Computation of the translucent illumination:
 
            float3 diffuseTranslucency = 
               attenuation * float3(_LightColor0) 
               * float3(_DiffuseTranslucentColor) 
               * max(0.0, dot(lightDirection, -normalDirection));
 
            float3 forwardTranslucency;
            if (dot(normalDirection, lightDirection) > 0.0) 
               // light source on the wrong side?
            {
               forwardTranslucency = float3(0.0, 0.0, 0.0); 
                  // no forward-scattered translucency
            }
            else // light source on the right side
            {
               forwardTranslucency = attenuation * float3(_LightColor0) 
                  * float3(_ForwardTranslucentColor) * pow(max(0.0, 
                  dot(-lightDirection, viewDirection)), _Sharpness);
            }
 
            // Computation of the complete illumination:
 
            return float4(ambientLighting 
               + diffuseReflection + specularReflection 
               + diffuseTranslucency + forwardTranslucency, 1.0);
         }
 
         ENDCG
      }
   }
}