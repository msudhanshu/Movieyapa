Shader "MyShader/Effect/FishEyeLens" {
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
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
 
         float4 frag(vertexOutput input) : COLOR
         {
         
            float4 color;
 			float2 m = float2(centerx,centery); //mouse position
  			float2 d = input.tex - m;
  			float r = length(d);
  				
			if ( r > _Radius) {
				color = tex2D(_OverlayTex, input.tex); 
			} else {
			
			
					// Thanks to Paul Bourke for these formulas; see
		        // http://paulbourke.net/miscellaneous/lenscorrection/
		        // and .../lenscorrection/lens.c
		        // Choose one formula to uncomment:
		        // SQUAREXY:
		      	//input.tex = m + float2(d.x * abs(d.x), d.y * abs(d.y));
		        // SQUARER:
		    	//input.tex = m + d * r; // a.k.a. m + normalize(d) * r * r
		        input.tex = m +  normalize(d) * r * r;
		        input.tex = m + d*sqrt(r);
		        // SINER:
		       // input.tex = m + normalize(d) * sin(r * 3.14159 * 0.5);
		        // ASINR:
		        // input.tex = m + normalize(d) * asin(r) / (3.14159 * 0.5);
		    
		          // col = texture2D(tex0, vec2(uv.x, -uv.y)).xyz;
        
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