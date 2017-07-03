// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Cutaways" {
   SubShader {
      Pass {
         Cull Off // turn off triangle culling, alternatives are:
         // Cull Back (or nothing): cull only back faces 
         // Cull Front : cull only front faces
 
         CGPROGRAM 
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         struct vertexInput {
            float4 vertex : POSITION;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posInObjectCoords : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.pos =  UnityObjectToClipPos(input.vertex);
            // mul(UNITY_MATRIX_MVP,input.vertex); 
            // mul(_Object2World,input.vertex); 
            output.posInObjectCoords = input.vertex; 
 
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
            if (input.posInObjectCoords.y > 0.0) 
            {
               discard; // drop the fragment if y coordinate > 0
            }
            
            //Depending on the hardware, this can be a quite expensive technique in the sense that 
            //rendering might perform considerably worse as soon as there is one shader that 
            //includes a discard instruction (regardless of how many fragments are actually discarded, 
            //just the presence of the instruction may result in 
            //the deactivation of some important optimizations).
            return float4(0.0, 1.0, 0.0, 1.0); // green
         }
 
         ENDCG  
      }
   }
}