// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/Effect/Emboss" {
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" { }
		//convolatoin kernal
		 // SHARPEN KERNEL
         //0 -1  0
        //-1  5 -1
         //0 -1  0
        
    }
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _MainTex_TexelSize;		
              					
			half4 Convolution(float2 texcoord0) {
			//	const half[9] KERNAL = { 
				const float3x3 KERNAL = { 
				    2.0, 0.0, 0.0,
				    0.0, -1.0, 0.0,
				    0.0, 0.0, -1.0
				   };
				const float NormalizeFactor = 1;
				const half3x3 texOffsetx = { 
				   -1,  0, 1,
				   -1,  0, 1,
				   -1,  0, 1	
				   };
				   
				const half3x3 texOffsety = { 
				    1,  1, 1,
				    0,  0, 0,
				   -1, -1,-1
				   };
			   	half3 sum=0;
				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 3; j++) {
              		 	 half3 color = tex2D(_MainTex, 
              		 	 texcoord0 + _MainTex_TexelSize*float2(texOffsetx[i][j], texOffsety[i][j])).rgb;
               			 sum += color * KERNAL[i][j];
       				 }
       			 }
				return half4(sum/NormalizeFactor,1);
			}

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
				//float2 uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
				o.texcoord0 = TRANSFORM_TEX ( i.texcoord0 , _MainTex);
				return o;
			}
			
			
			half4 frag(fragmentInput input) : COLOR {
				half4 original =  tex2D(_MainTex, input.texcoord0);
				//return original;
				
				return Convolution(input.texcoord0);		
			}
			
			ENDCG
		}
	}
}