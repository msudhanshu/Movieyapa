﻿Shader "billboards" {
   Properties {
      _MainTex ("Texture Image", 2D) = "white" {}
   }
   SubShader {
      Pass {   
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         // User-specified uniforms            
         uniform sampler2D _MainTex;        
 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 tex : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
         };
  
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.pos = mul(UNITY_MATRIX_P, 
              mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) - //input.vertex
               float4(input.vertex.x, input.vertex.z, 0, 0.0)
              );
              //Unity performs the scaling on the object coordinates before they are sent to the vertex shader 
              //(unless all three scale factors are positive and equal, 
              //then the scaling is specified by 1.0 / unity_Scale.w). 
              
            output.tex = input.tex;
 
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            return tex2D(_MainTex, float2(input.tex.xy));   
         }
 
         ENDCG
      }
   }
}