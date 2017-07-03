// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/Effect/EdgeDetect" {
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" { }
		_Treshold ("_Treshold", Range(0.0001,0.1)) = 0.01
		//convolatoin kernal
		// 0 0 0
		// 0 2 0
		// -1 0 -1
    }
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _MainTex_TexelSize;
			uniform float _Treshold;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			struct fragmentInput{
				float4 position : SV_POSITION;
				float2 texcoord0[3] : TEXCOORD0;
				//float2 texcoord_lefttexel : TEXCOORD1;
				//float2 texcoord_righttexel : TEXCOORD2;
				
			};

			fragmentInput vert(vertexInput i){
				fragmentInput o;
				o.position = UnityObjectToClipPos (i.vertex);
				//float2 uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
				float2 uv = TRANSFORM_TEX ( i.texcoord0 , _MainTex);
				o.texcoord0[0] = uv;
				o.texcoord0[1] = uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y);
				o.texcoord0[2] = uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y);
				//o.texcoord_lefttexel  = o.texcoord0 + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y);
				//o.texcoord_righttexel = o.texcoord0 + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y);
				return o;
			}
			
			
			half4 frag(fragmentInput i) : COLOR {
	
				half4 original =  tex2D(_MainTex, i.texcoord0[0]);
				
							// a very simple cross gradient filter
				half3 p1 = original.rgb;
				half3 p2 = tex2D( _MainTex, i.texcoord0[0] + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y)).rgb;
				half3 p3 = tex2D( _MainTex, i.texcoord0[2] ).rgb;
				
				half3 diff = p1 * 2 - p2 - p3;
				half len = dot(diff,diff);
				if( len >= _Treshold )
					original.rgb = 0;
							
				return original;
			}
			
			
			ENDCG
		}
	}
}