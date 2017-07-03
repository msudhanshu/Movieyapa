// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MeshPie" {
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" { }
		startX ("startX", Float) = 0.0
		startY ("startY", Float ) = 0.0
		endX ("endX", Float) = 1.0
		endY ("endY", Float ) = 1.0
    }
    
	SubShader {
		Pass {
		 Cull Front // cull only back faces
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float startX;
			uniform float startY;
			uniform float endX;
			uniform float endY;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			struct fragmentInput{
				float4 position : SV_POSITION;
				float2 texcoord0 : TEXCOORD0;
				float4 posInObjectCoords : TEXCOORD1;
			};

			fragmentInput vert(vertexInput i){
				fragmentInput o;
				o.position = UnityObjectToClipPos (i.vertex);
				o.texcoord0 = TRANSFORM_TEX ( i.texcoord0 , _MainTex);
				o.posInObjectCoords = i.vertex; 
				return o;
			}
			
			
			float4 frag(fragmentInput i) : COLOR {
				//if inside the boundary then only render it else discard
				//	if(i.texcoord0.x < startX || i.texcoord0.y < startY || i.texcoord0.x > endX || i.texcoord0.y > endY) {
				//	discard;
				//	}
				float2 discardParam = float2(i.posInObjectCoords.x, i.posInObjectCoords.z);
				//float2 discardParam = discardParam = i.texcoord0.xy;
				if(discardParam.x < startX || discardParam.y < startY || discardParam.x > endX || discardParam.y > endY) {
				discard;
				}
					
				return float4(1.0, 0.0, 0.0, 1.0); // red
				//return tex2D(_MainTex, i.texcoord0);
			
			}
			ENDCG
		}
		
				Pass {
				 Cull Back // cull only back faces
				 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float startX;
			uniform float startY;
			uniform float endX;
			uniform float endY;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			struct fragmentInput{
				float4 position : SV_POSITION;
				float2 texcoord0 : TEXCOORD0;
				float4 posInObjectCoords : TEXCOORD1;
			};

			fragmentInput vert(vertexInput i){
				fragmentInput o;
				o.position = UnityObjectToClipPos (i.vertex);
				o.texcoord0 = TRANSFORM_TEX ( i.texcoord0 , _MainTex);
				o.posInObjectCoords = i.vertex; 
				return o;
			}
			
			
			float4 frag(fragmentInput i) : COLOR {
				//if inside the boundary then only render it else discard
				float2 discardParam = float2(i.posInObjectCoords.x, i.posInObjectCoords.z);
				//discardParam = i.texcoord0.xy;
				if(discardParam.x < startX || discardParam.y < startY || discardParam.x > endX || discardParam.y > endY) {
				discard;
				}
				
				return tex2D(_MainTex, i.texcoord0);
			
			}
			ENDCG
		}
		
	}
}