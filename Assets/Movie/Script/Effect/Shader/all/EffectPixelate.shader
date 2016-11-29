Shader "MyShader/Effect/Pixelate" {
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" { }
		//convolatoin kernal
		 // SHARPEN KERNEL
         //0 -1  0
        //-1  5 -1
         //0 -1  0
        pixelcount ("PixelCount", Float) = 10
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
            uniform float pixelcount;
              					
			half4 Convolution(float2 texcoord0, float s) {
			//	const half[9] KERNAL = { 
				const half3x3 KERNAL = { 
				    1,1,1,
				   	1,1,1,
				    1,1,1
				   };
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
				return half4(sum/9.0,1);
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
				o.position = mul (UNITY_MATRIX_MVP, i.vertex);
				//float2 uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
				o.texcoord0 = TRANSFORM_TEX ( i.texcoord0 , _MainTex);
				return o;
			}
			
			
			float4 frag(fragmentInput input) : COLOR {
				half4 original;//  = tex2D(_MainTex, input.texcoord0);
				float sx = fmod(2.0*pixelcount*input.texcoord0.x,2.0);
				if (sx<1.0 || fmod(2.0*pixelcount*input.texcoord0.y,2.0)<1.0) {
					 original =  Convolution(input.texcoord0,sx);
				}
				
				return original;
				
				//return Convolution(input.texcoord0);		
			}
			
			ENDCG
		}
	}
}