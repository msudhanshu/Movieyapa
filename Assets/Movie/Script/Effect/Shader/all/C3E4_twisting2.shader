Shader "Custom/C3E4_twisting2" {
	Properties
	{
		twisting ("twisting", Float) = 0.5
		_color ("color", Color) = (1,1,1,1)
    }
	SubShader 
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};
			// This differs from the original C3E4 example as I exposed the _color param instead of letting Unity provide it itself
			v2f vert(	float4 pos : POSITION,
						uniform float4 _color,
						uniform float twisting)
			{
				v2f o;
				//method 2
				//pos = mul (UNITY_MATRIX_MVP, pos);
				float angle = twisting * length(pos);
				float cosLength, sinLength;
				sincos(angle, sinLength, cosLength);
				pos[0] = cosLength * pos[0] +
						-sinLength * pos[2];
				pos[2] = sinLength * pos[0] +
						cosLength * pos[2];
				//o.pos[0] =  pos[0];
				//o.pos[1] = pos[1]; 
				pos[1] = pos[1];
				pos[3] = 1;
				o.pos = mul (UNITY_MATRIX_MVP, pos);
				o.color = _color;
				return o;
 
			}
			// regular passthrough fragment shader
			float4 frag(v2f i) : COLOR
			{
				return i.color;
			}
		ENDCG
		}
	}	
}