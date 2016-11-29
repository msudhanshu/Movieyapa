// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _World2Object', a built-in variable


﻿Shader "Custom/BasicLight"
{
	Properties
	{
		Ke ("Ke", Vector) = (-0.5,0,0,0)
		Ka ("Ka", Vector) = (-0.5,0,0,0)
		Kd ("Kd", Vector) = (-0.5,0,0,0)
		Ks ("Ks", Vector) = (-0.5,0,0,0)
		shininess ("shininess", float) = 0.5
		lightColor ("lightColor", Color) = (0.5,0,0,0)
    }
	SubShader 
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex C5E1v_basicLight
			#pragma fragment frag
 			#include "UnityCG.cginc"
 					
 			//user defined variable
	          uniform float3 Ke;
	
	          uniform float3 Ka;
	
	          uniform float3 Kd;
	
	          uniform float3 Ks;
	
	          uniform float  shininess;
			              
			  uniform float3 lightColor,
			  //uniform float3 lightColor,
			              
			   //unity defined variable
			   uniform float4 _LightColor0;
			   
			   //if using unity3.5 .. not required for unity 4
			     // float4x4 _Object2World;
			     // float4x4 _World2Object;           
			   float4 _WorldSpaceLightPos0;
			   
			   
				void C5E1v_basicLight(float4 position  : POSITION,
			
			                      float3 normal    : NORMAL,

			                  out float4 oPosition : SV_POSITION,
			
			                  out float4 color     : COLOR,
			
			
			              uniform float3 globalAmbient,
			
			              uniform float3 lightPosition,
			
			              uniform float3 eyePosition,
			)
			
			
			
			{
			
			  oPosition = mul(UNITY_MATRIX_MVP, position);
			
			
			
			  float3 P = position.xyz;
			
			  float3 N = normal;
			
			
			
			  // Compute the emissive term
			
			  float3 emissive = Ke;
			
			
			
			  // Compute the ambient term
			
			  float3 ambient = Ka * globalAmbient;
			
			
			
			  // Compute the diffuse term
			
			  float3 L = normalize(_WorldSpaceLightPos0 - P);
			
			  float diffuseLight = max(dot(N, L), 0);
			
			  float3 diffuse = Kd * lightColor * diffuseLight;
			
			
			
			  // Compute the specular term
			
			  float3 V = normalize(eyePosition - P);
			
			  float3 H = normalize(L + V);
			
			  float specularLight = pow(max(dot(N, H), 0),
			
			                            shininess);
			
			  if (diffuseLight <= 0) specularLight = 0;
			
			  float3 specular = Ks * lightColor * specularLight;
			
			
			
			  color.xyz = emissive + ambient + diffuse + specular;
			
			  color.w = 1;
			
			}


			void frag(	
			float4 icolor : COLOR
			out float4 color : COLOR)
			{
				icolor;
			}

		ENDCG
		}
	}	
}