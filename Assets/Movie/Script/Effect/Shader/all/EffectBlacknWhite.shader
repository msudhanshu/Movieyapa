Shader "MyShader/TextureCoordinates/Effect BlacknWhite" {
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" { }
		_Lumcoeff ("_Lumcoeff", Vector) = (0.299, 0.587, 0.114, 0)
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
			uniform float4 _Lumcoeff;
			
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
				float4 col = tex2D(_MainTex, i.texcoord0);
				 float lum = dot(col, _Lumcoeff);
				  if (0.5 < lum) {
				    return  float4(1, 1, 1, 1);;
				  } else {
				    return float4(0, 0, 0, 1);
				  }     
			}
			
			
			ENDCG
		}
	}
}