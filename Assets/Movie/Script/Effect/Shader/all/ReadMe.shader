//Note
//1.If you want to make sure that a fragment shader receives one exact, non-interpolated value by a vertex shader, you have to make sure that the vertex shader writes the same value to the vertex output parameters for all vertices of a triangle.
//2. //Depending on the hardware, this can be a quite expensive technique in the sense that 
            //rendering might perform considerably worse as soon as there is one shader that 
            //includes a discard instruction (regardless of how many fragments are actually discarded, 
            //just the presence of the instruction may result in 
            //the deactivation of some important optimizations).
            
//cookies
//projector
//silhouette
//gloss, bloom with extra texture
// fire, fur
// tv screen shaders 

//Note that the alpha test and the discard instruction are rather slow on some platforms,
// in particular on mobile devices. Thus, blending is often a more efficient alternative.

//1. Additive : example for additive blending are double exposures ,
				// Blend SrcAlpha One
//2. Multiplicative : An example for multiplicative blending in photography is the use of multiple uniform grey filters: the order in which the filters are put onto a camera doesn't matter for the resulting attenuation of the image. In terms of the rasterization of triangles, the image corresponds to the contents of the framebuffer before the triangles are rasterized, while the filters correspond to the triangles.
				//Blend Zero SrcAlpha 
//Blend SrcAlpha OneMinusSrcAlpha     // Alpha blending
//Blend One One                       // Additive
//Blend OneMinusDstColor One          // Soft Additive
//Blend DstColor Zero                 // Multiplicative
//Blend DstColor SrcColor             // 2x Multiplicative