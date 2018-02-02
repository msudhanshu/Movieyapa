// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/Effect/EffectCircleHole" {
   Properties {
        _MainTex ("RGBA Texture Image", 2D) = "white" {} 
        _OverlayTex ("Overlay", 2D) = "white" {} 
		_Radius ("Radius", Range(0,1.5)) = 0.5
		centerx ("CenterX", Range(0,1)) = 0.5
		centery ("CenterY", Range(0,1) ) = 0.5
		
   }
   SubShader {
      Pass {    
       Tags { "LightMode" = "Vertex" }//otherwise no light related values will be filled
         Cull Off // since the front is partially transparent, 
            // we shouldn't cull the back
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 		#include "UnityCG.cginc"
 
        uniform sampler2D _MainTex;    
        uniform float4 _MainTex_ST;  
        uniform sampler2D _OverlayTex;    
        uniform float4 _OverlayTex_ST;  
 		uniform float _Radius;
 		uniform float centerx;
		uniform float centery;
		
         struct vertexInput {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float2 tex : TEXCOORD0;
          //  float2 overlayTex : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
            // pass the normal further to fragment shader if u want
            //output.tex = input.texcoord.xy;
            output.tex  = TRANSFORM_TEX (input.texcoord, _MainTex);
         //   output.overlayTex  = TRANSFORM_TEX (input.texcoord, _OverlayTex);
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
 
         float4 frag(vertexOutput input) : COLOR
         {
         	//_Rate = fmod(_Time.w,100)*_Rate;
 			float2 texcord = input.tex;
  			float2 texcord_wrt_center = texcord - float2(centerx,centery);
            float4 color;
			if ( length(texcord_wrt_center) > _Radius) {
				color = tex2D(_OverlayTex, input.tex); 
			} else {
				color = tex2D(_MainTex, input.tex);  
			} 
			return color;
         }
 
         ENDCG
      }
   }
 
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Unlit/Transparent Cutout"
}