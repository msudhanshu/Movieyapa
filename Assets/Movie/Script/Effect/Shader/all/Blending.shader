// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//1. Additive : example for additive blending are double exposures ,
				// Blend SrcAlpha One
//2. Multiplicative : An example for multiplicative blending in photography is the use of multiple uniform grey filters: the order in which the filters are put onto a camera doesn't matter for the resulting attenuation of the image. In terms of the rasterization of triangles, the image corresponds to the contents of the framebuffer before the triangles are rasterized, while the filters correspond to the triangles.
				//Blend Zero SrcAlpha 
//Blend SrcAlpha OneMinusSrcAlpha     // Alpha blending
//Blend One One                       // Additive
//Blend OneMinusDstColor One          // Soft Additive
//Blend DstColor Zero                 // Multiplicative
//Blend DstColor SrcColor             // 2x Multiplicative

Shader "Blending" {
   SubShader {
      Tags { "Queue" = "Transparent" } 
         // draw after all opaque geometry has been drawn
      Pass {
         Cull Front // first pass renders only back faces 
             // (the "inside")
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
 
         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
 
         float4 vert(float4 vertexPos : POSITION) : SV_POSITION 
         {
            return UnityObjectToClipPos(vertexPos);
         }
 
         float4 frag(void) : COLOR 
         {
            return float4(1.0, 0.0, 0.0, 0.3);
               // the fourth component (alpha) is important: 
               // this is semitransparent red
         }
 
         ENDCG  
      }
 
      Pass {
         Cull Back // second pass renders only front faces 
             // (the "outside")
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
 
         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
 
         float4 vert(float4 vertexPos : POSITION) : SV_POSITION 
         {
            return UnityObjectToClipPos(vertexPos);
         }
 
         float4 frag(void) : COLOR 
         {
            return float4(0.0, 1.0, 0.0, 0.3);
               // the fourth component (alpha) is important: 
               // this is semitransparent green
         }
 
         ENDCG  
      }
   }
}