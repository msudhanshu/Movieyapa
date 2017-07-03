// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/LightAmbient_PerVertex" {
      Properties {
        _MainTex ("Texture 1", 2D) = "white" {}
        _Ambient("Ambient Color",Color) = (0.3,0.3,0.3,1)
      }
      SubShader {
      Tags { "RenderType"="Opaque"}
      Pass {
           Tags { "LightMode" = "Vertex" }//otherwise no light related values will be filled
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
           #include "UnityCG.cginc"
   
           sampler2D _MainTex;
           fixed4 _Ambient;
           half4 _MainTex_ST;
   
           struct v2f{
               fixed4 position: POSITION;
               half2 uv_MainTex:TEXCOORD0;
               fixed4 color: COLOR;
           };
   
           v2f vert (appdata_base ab) {
               v2f o;
               o.position = UnityObjectToClipPos(ab.vertex);
               o.uv_MainTex = TRANSFORM_TEX(ab.texcoord.xy , _MainTex);
               o.color.xyz = _Ambient.xyz;
               return o;
           }
   
           fixed4 frag(v2f i):COLOR{
              fixed4 c = tex2D(_MainTex, i.uv_MainTex);   
              return  c * i.color;
           }
           ENDCG
      }
      }
Fallback "Mobile/Diffuse"
}