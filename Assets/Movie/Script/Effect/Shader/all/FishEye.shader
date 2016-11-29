Shader "MyShader/Effect/FishEye" {
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
            const float PI = 3.14159;
              float aperture = 178.0;
			  float apertureHalf = 0.5 * aperture * (PI / 180.0);
			  float maxFactor = sin(apertureHalf);
   		
   		
   			float2 m = float2(centerx,centery); //mouse position
  			float2 d = input.tex - m;
  			float r = length(d);
  				
	
			
   			d = 2.0 * input.tex - 1.0;
  			r = length(d);
  			
            if ( r < _Radius) 
            		
			//if (r < (2.0-maxFactor))
			  {
			    r = length(d * maxFactor);
			    float z = sqrt(1.0 - r * r);
			    float rad = atan(r, z) / PI;
			    float phi = atan(d.y, d.x);
			    
			    input.tex.x = rad * cos(phi) + 0.5;
			    input.tex.y = rad * sin(phi) + 0.5;
			    color = tex2D(_MainTex, input.tex); 
			  } else 
				color = tex2D(_OverlayTex, input.tex); 
			
			return color;
         }
 
         ENDCG
      }
   }
 
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Unlit/Transparent Cutout"
}