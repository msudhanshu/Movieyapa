Shader "Effect/Zoom" {
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" { }
		_Rate ("Rate", Range(0,1)) = 0.5
		centerx ("CenterX", Range(0,1)) = 0.5
		centery ("CenterY", Range(0,1) ) = 0.5
    }
    
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Rate;
			uniform float centerx;
			uniform float centery;
			
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
				i.texcoord0.x += 	_Rate*(centerx-i.texcoord0.x);
				i.texcoord0.y += 	_Rate*(centery-i.texcoord0.y);
				return tex2D(_MainTex, i.texcoord0);
			
			}
			ENDCG
		}
	}
}