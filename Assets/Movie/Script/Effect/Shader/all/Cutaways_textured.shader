// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Cutaways_textured" {
   Properties {
      _height ("Height", Float) = 0
      _baseZpos ("_baseZpos", Float) = 0
      _MainTex ("Base Texture", 2D) = "white" {} 
         // user-specified RGBA color including opacity
   }
   SubShader {
      Pass {
         Cull Off // turn off triangle culling, alternatives are:
         // Cull Back (or nothing): cull only back faces 
         // Cull Front : cull only front faces
 
         CGPROGRAM 
 
         #pragma vertex vert  
         #pragma fragment frag 
 		 #include "UnityCG.cginc"
 			
 		 uniform sampler2D _MainTex;
 		 uniform float4 _MainTex_ST;
 		 uniform float _baseZpos;
         float _height;
 		 
         struct vertexInput {
            float4 vertex : POSITION;
            //TODO : if want light then go for surface shader
            //float3 normal : NORMAL; // for light calculation
            float2 texCoord : TEXCOORD0;
         };
         
         struct vertexOutput {
            float4 pos : SV_POSITION;
			float2 texCoord : TEXCOORD0;
            float cullableHeight;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.pos =  UnityObjectToClipPos(input.vertex);
            // mul(UNITY_MATRIX_MVP,input.vertex); 
            // mul(_Object2World,input.vertex); 
            //1. input.vertex
            //2. mul(_Object2World,input.vertex)
            //3. mul(_Object2World,input.vertex) - mul(_Object2World,float4(0.0,0.0,0.0,1.0));
            
            output.cullableHeight = (mul(unity_ObjectToWorld,input.vertex) - float4(0.0,_baseZpos,0.0,1.0)).y;//input.vertex; 
 			output.texCoord = TRANSFORM_TEX (input.texCoord, _MainTex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
         	//FIXME : DISCARD commond is expensive
            if (input.cullableHeight > _height) 
            {
               discard; // drop the fragment if y coordinate > 0
            }
            
            //Depending on the hardware, this can be a quite expensive technique in the sense that 
            //rendering might perform considerably worse as soon as there is one shader that 
            //includes a discard instruction (regardless of how many fragments are actually discarded, 
            //just the presence of the instruction may result in 
            //the deactivation of some important optimizations).
            
            return tex2D(_MainTex, input.texCoord);
         }
 
         ENDCG  
      }
   }
}