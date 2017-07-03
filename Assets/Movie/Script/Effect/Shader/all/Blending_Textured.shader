// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Note that the alpha test and the discard instruction are rather slow on some platforms, in particular on mobile devices. Thus, blending is often a more efficient alternative.
Shader "Blending_Textured" {
	Properties {
	_MainTex ("Front Texture Image", 2D) = "white" {} 
	_BackTex ("Back Texture Image", 2D) = "white" {} 
	_MainAlpha ("Front Texture Alpha ", Range(0.0,1.0)) = 0.5
	_BackAlpha ("Back Texture Alpha ", Range(0.0,1.0)) = 0.5
	}
	
   SubShader {
      Tags { "Queue" = "Transparent" } 
         // draw after all opaque geometry has been drawn
      Pass {
         Cull Front // first pass renders only back faces 
             // (the "inside")
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
 
         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag

 		uniform sampler2D _BackTex;  
  		uniform float _BackAlpha;
  		struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
         };
         
 
         vertexOutput vert(float4 vertexPos : POSITION, float4 texcoord : TEXCOORD0) 
         {
         	vertexOutput output;
         	output.tex = texcoord;
         	output.pos = UnityObjectToClipPos(vertexPos);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
         
       		  float4 textureColor = tex2D(_BackTex, float2(input.tex));  
            //return textureColor; //if want to use image alpha
        	 return float4(float3(textureColor),_BackAlpha);
          //  return float4(0.0, 0.0, 1.0, 0.3);
               // the fourth component (alpha) is important: 
               // this is semitransparent red
         }
 
         ENDCG  
      }
 
      Pass {
         Cull Back // second pass renders only front faces 
             // (the "outside")
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
 
         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
 
 		uniform sampler2D _MainTex;  
  		uniform float _MainAlpha;
  		
          struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
         };
         
 
         vertexOutput vert(float4 vertexPos : POSITION, float4 texcoord : TEXCOORD0) 
         {
         	vertexOutput output;
         	output.tex = texcoord;
         	output.pos = UnityObjectToClipPos(vertexPos);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
                     
       		float4 textureColor = tex2D(_MainTex, float2(input.tex));  
            //return textureColor; //if want to use image alpha
        	 return float4(float3(textureColor),_MainAlpha);
        	 //return float4(0.0, 1.0, 0.0, 0.8);
               // the fourth component (alpha) is important: 
               // this is semitransparent green
         }
 
         ENDCG  
      }
   }
}