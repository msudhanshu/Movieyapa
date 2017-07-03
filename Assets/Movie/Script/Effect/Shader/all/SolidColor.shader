// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/SolidColor" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,0.5)
}
SubShader {
    Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
float4 _Color;

float4 vert(float4 v:POSITION) : SV_POSITION {
	return UnityObjectToClipPos (v);
}
		
fixed4 frag() : COLOR {
	return _Color;//fixed4(1.0,1.0,0.0,1.0);
}

ENDCG

    }
}
Fallback "VertexLit"
} 