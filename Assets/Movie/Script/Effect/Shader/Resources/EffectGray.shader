Shader "MyShader/Effect/Gray" {
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" { }
		//NTSC formula :intensity = 0.2989*red + 0.5870*green + 0.1140*blue // or simple avg (1/3,1/3,
		_NTSCValue ("NTSC converstion constant", Vector) = (0.2989, 0.5870, 0.1140,0)
    }
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _NTSCValue;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			struct fragmentInput{
				float4 position : SV_POSITION;
				float2 texcoord0 : TEXCOORD0;
			};

			fragmentInput vert(vertexInput i){
				fragmentInput o;
				o.position = mul (UNITY_MATRIX_MVP, i.vertex);
				o.texcoord0 = TRANSFORM_TEX ( i.texcoord0 , _MainTex);
				return o;
			}
			
			
			float4 frag(fragmentInput i) : COLOR {
				return dot(_NTSCValue,tex2D(_MainTex, i.texcoord0));
				//return float4(grayval,grayval,grayval,1);
			}
			
			
			ENDCG
		}
	}
}