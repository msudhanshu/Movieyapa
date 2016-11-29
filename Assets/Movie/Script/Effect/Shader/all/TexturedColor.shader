Shader "MyShader/TexturedColor" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,0.5)
    _MainTex ("Texture", 2D) = "white" { }
}
SubShader {
    Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		
		#include "UnityCG.cginc"
		
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		
		struct v2f {
		    float4  pos : SV_POSITION;
		    float2  uv : TEXCOORD0;
		};
		
		
		
		v2f vert (appdata_base v)
		{
		    v2f o;
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		    
		    //From unityCG.cginc : Transforms 2D UV by scale/bias property
			//  #define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
			//  when user enters TRANSFORM_TEX(texUvCoords, texName) process it as (texUvCoords.xy * texName_ST.xy + texName_ST.zw)
			//  This way, we can use the offset - tiling properties of a material
			//  Reference : http://unity3d.com/support/documentation/Components/class-Material.html
			
			//Texture position in a mesh is given by the variable texcoord0, which is set for each vertex. 
			//The vertex and fragment shader can modify this variable.
			// You should select in which shader you modify texcoord0, depending of the variables needed
			// for the operation, the number of vertex in the scene and the screen resolution. 
			//You should select the modification of texcoord0 values looking for the smallest number of executions.
			
		    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		    return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
		    half4 texcol = tex2D (_MainTex, i.uv);
		    return texcol * _Color;
		}
ENDCG

    }
}
Fallback "VertexLit"
} 