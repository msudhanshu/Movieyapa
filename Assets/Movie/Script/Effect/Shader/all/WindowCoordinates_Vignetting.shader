Shader "MyShader/WindowCoordinates/Vignetting" {
	Properties
	{
		centerx ("CenterX", Range(0.0,1.0)) = 0.5
		centery ("CenterY", Range(0.0,1.0)) = 0.5
		radius ("Radius", Range(0.0,1.0)) = 0.5
    }
	SubShader {
		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			uniform float centerx;
			uniform float centery;
			uniform float radius;
			//uniform float2 center = float2(centerx,centery);
			float4 vert(appdata_base v) : POSITION {
				return mul (UNITY_MATRIX_MVP, v.vertex);
			}

			float4 frag(float4 sp:WPOS) : COLOR {
				float2 wcoord = sp.xy/_ScreenParams.xy;
				const float2 center = float2(centerx,centery);
				float vig = clamp((1.0/radius)*length(wcoord-center),0.0,1.0);
				return lerp (float4(wcoord,0.0,1.0),float4(0.3,0.3,0.3,1.0),vig);
			}
			ENDCG
		}
	}
}