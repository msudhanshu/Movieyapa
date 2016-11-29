Shader "MyShader/TextureCoordinates/CirclesMask" {
	Properties {
		_CirclesX ("Circles in X", Float) = 20
		_CirclesY ("Circles in Y", Float) = 10
		_Fade ("Fade", Range (0.1,1.0)) = 0.5
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			uniform float _CirclesX;
			uniform float _CirclesY;
			uniform float _Fade;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			struct fragmentInput{
				float4 position : SV_POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			fragmentInput vert(vertexInput i){
				fragmentInput o;
				o.position = mul (UNITY_MATRIX_MVP, i.vertex);
				o.texcoord0 = i.texcoord0;
				return o;
			}
			
			
			float4 frag(fragmentInput i) : COLOR {
				float2 wcoord = i.texcoord0.xy;
				float4 color;
				if ( length( fmod(float2(_CirclesX*wcoord.x,_CirclesY*wcoord.y),2.0)-1.0) <_Fade) {
					color = float4(wcoord,0.0,1.0);
				} else {
					color = float4(0.3,0.3,0.3,1.0);
				} 
				return color;
			}
			ENDCG
		}
	}
}