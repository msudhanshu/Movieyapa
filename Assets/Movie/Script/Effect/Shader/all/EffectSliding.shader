Shader "MyShader/TextureCoordinates/Effect Sliding Color" {
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" { }
		_SliceCount ("Slice Count", Float) = 3
		_SlideAmount ("Slide amount", Range(0,0.5)) = 0.05
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
			uniform float _SliceCount;
			uniform float _SlideAmount;
			
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
				//to zoom , so that top bottom slided reagion is not visible
				i.texcoord0 += 	2*_SlideAmount*(0.5-i.texcoord0);
				
				if (fmod(_SliceCount*i.texcoord0.x,2.0)<1.0) 
				{
				i.texcoord0 = 	i.texcoord0 + float2(0,_SlideAmount);
				} else	
				i.texcoord0 = 	i.texcoord0 - float2(0,_SlideAmount);
				
				return tex2D(_MainTex, i.texcoord0);
			}
			
			
			ENDCG
		}
	}
}