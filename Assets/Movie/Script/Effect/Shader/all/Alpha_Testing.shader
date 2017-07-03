// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Note that the alpha test and the discard instruction are rather slow on some platforms, in particular on mobile devices. Thus, blending is often a more efficient alternative.
Shader "Alpha_Testing" {
   Properties {
      _MainTex ("RGBA Texture Image", 2D) = "white" {} 
      _Cutoff ("Alpha Cutoff", Float) = 0.5
   }
   SubShader {
      Pass {	
         Cull Off // since the front is partially transparent, 
            // we shouldn't cull the back
         AlphaTest Greater [_Cutoff] // specify alpha test: 
            // fragment passes if alpha is greater than _Cutoff 
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         uniform sampler2D _MainTex;    
         uniform float _Cutoff;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.tex = input.texcoord;
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            return tex2D(_MainTex, float2(input.tex));  
         }
 
         ENDCG
      }
   }
 
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Unlit/Transparent Cutout"
}