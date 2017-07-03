// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MetaballShader" {
    Properties {
        _MainTex ("Diffuse (RGB) Gloss (A)", 2D) = "white" {}
    }
	SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
        LOD 200
        Cull Off
        CGPROGRAM
        // Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
        #pragma exclude_renderers gles
        #pragma vertex vert
        #pragma surface surf BlinnPhong
        #include "UnityCG.cginc"
        #pragma target 3.0

        sampler2D _MainTex;
        float _FloatTime;

        struct Input {
            float2 uv_MainTex;
            float4 pos : SV_POSITION;
            float3 normal : TEXCOORD;
        };

        float3 getDeform(float3 p)
        {
        const float PI = 3.14159;
        float tFloatTime = _FloatTime;//cos(fmod(_Time.x*0.5,2*PI));//
        float FloatTime = 5 * tFloatTime;
       
            return (
            tFloatTime*
            (float3(p.xy, 0.3) * 
            (  cos(FloatTime*4 + p.z) + sin(p.x * 10 + FloatTime * 9) + cos(p.y * 10 + FloatTime*3)  )
            		) / 10  + 
                float3(0,cos(FloatTime*4 + p.z + cos(p.z + FloatTime)) / 6, 0)
                );
        }

        void vert (inout appdata_full v, out Input o) {
            float4 p = v.vertex;
            p.xyz += getDeform(p);
            v.vertex = p;
            o.pos = UnityObjectToClipPos (p);
            o.normal = v.vertex;
            v.normal = v.vertex;
        }
        
        void surf(Input IN, inout SurfaceOutput o)  
        { 
            o.Albedo = float4(1,0,0,1);
            o.Alpha = 1;
            o.Specular = 1; 
            o.Gloss = 1;
        }

        ENDCG
    }
    FallBack "Diffuse"
}