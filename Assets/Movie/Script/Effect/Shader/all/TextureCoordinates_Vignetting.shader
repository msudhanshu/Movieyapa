Shader "MyShader/TextureCoordinates/Vignetting" {
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

			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			struct fragmentInput{
				float4 position : SV_POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			uniform float centerx;
			uniform float centery;
			uniform float radius;

			fragmentInput vert(vertexInput i){
				fragmentInput o;
				o.position = mul (UNITY_MATRIX_MVP, i.vertex);
				o.texcoord0 = i.texcoord0;
				return o;
			}
			
			
			float4 frag(fragmentInput i) : COLOR {
				float2 wcoord = i.texcoord0.xy;
				const float2 center = float2(centerx,centery);
				float vig = clamp((1.0/radius)*length(wcoord-center),0.0,1.0);
				return lerp (float4(wcoord,0.0,1.0),float4(0.3,0.3,0.3,1.0),vig);
			}
			ENDCG
		}
	}
}