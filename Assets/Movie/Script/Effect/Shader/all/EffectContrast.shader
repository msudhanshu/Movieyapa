// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/Effect/Contrast" {
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" { }
		//NTSC formula :intensity = 0.2989*red + 0.5870*green + 0.1140*blue // or simple avg (1/3,1/3,
		_LumCoeff ("LumCoeff converstion constant", Vector) = (0.2125, 0.7154, 0.0721,0)
		_Contrast ("Contrast ",Range(0,1)) = 0.5
    }
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _LumCoeff;
			uniform float _Contrast;
			
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
				o.position = UnityObjectToClipPos (i.vertex);
				o.texcoord0 = TRANSFORM_TEX ( i.texcoord0 , _MainTex);
				return o;
			}
			
			
			float4 frag(fragmentInput i) : COLOR {
				float3 color = tex2D(_MainTex, i.texcoord0).rgb;
				float3 AvgLumin = 0.5;
				float3 intensity = float3(dot(color, _LumCoeff.rgb));
				 // could substitute a uniform for this 1. and have variable saturation
     		   float3 satColor = lerp(intensity, color, 1.);
     		   float3 conColor = lerp(AvgLumin, satColor, _Contrast);

     		   return float4(conColor, 1);
			}
			
			
			ENDCG
		}
	}
}