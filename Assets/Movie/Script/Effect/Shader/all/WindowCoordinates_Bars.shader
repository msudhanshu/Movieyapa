Shader "MyShader/WindowCoordinates/Bars" {
	Properties
	{
		barcount ("BarCount", Float) = 10
    }
    
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct vertOut {
				float4 pos:SV_POSITION;
				float4 scrPos;
			};
			uniform float barcount;
			
			vertOut vert(appdata_base v) {
				vertOut o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				
				//ComputeScreenPos : Function define in the UnityCG.cginc file, 
				//this functions return the screen position for the fragment shader. 
				//The difference with the previous example were a WPOS semantic variable was used, 
				//this function is multiplatform and it does not need target 3.0.

				o.scrPos = ComputeScreenPos(o.pos);
				return o;
			}

			fixed4 frag(vertOut i) : COLOR0 {
				float2 wcoord = (i.scrPos.xy/i.scrPos.w);
				fixed4 color;

				if (fmod(2.0*barcount*wcoord.x,2.0)<1.0) {
					color = fixed4(wcoord.xy,0.0,1.0);
				} else {
					color = fixed4(0.3,0.3,0.3,1.0);
				}
				return color;
			}

			ENDCG
		}
	}
}