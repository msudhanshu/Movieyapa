Shader "MyShader/TextureCoordinates/Bar" {
	Properties
	{
		barcount ("BarCount", Float) = 10
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

			uniform float barcount;

			fragmentInput vert(vertexInput i){
				fragmentInput o;
				o.position = mul (UNITY_MATRIX_MVP, i.vertex);
				o.texcoord0 = i.texcoord0;
				return o;
			}
			
			
			float4 frag(fragmentInput i) : COLOR {
				fixed4 color;
				if (fmod(2.0*barcount*i.texcoord0.x,2.0)<1.0) {
					color = float4(i.texcoord0.xy,0.0,1.0);
				} else {
					color = fixed4(0.3,0.3,0.3,1.0);
				}
				
				//color=float4(i.texcoord0.xy,0.0,1.0);
				return color;
			}
			ENDCG
		}
	}
}