// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Effect DoubleVision"
{
	Properties
	{
		_MainTex ("Diffuse", 2D) = "white" {}
		//Unity does not support 2-elements vectors. But that's OK since the implicit casts work just fine.
		_leftSeparation ("Left Separation", Vector) = (-0.02,0,0,0)
		_rightSeparation ("Right Separation", Vector) = (0.02,0,0,0)
		//Let the user control the lerp value
		_lerpValue ("Linear Interpolation Value", Range(0.0 ,1.0)) = 0.5
		_speed ("Speed", Float) = 0.5
    }
    
	SubShader 
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 			#include "UnityCG.cginc"
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
 			uniform float _speed;
			uniform float2 _leftSeparation;
			uniform float2 _rightSeparation;
 			
			void vert(	float4 pos : POSITION,
						float2 texCoord : TEXCOORD0,
						out float4 oPos : SV_POSITION,
						out float2 leftTexCoord : TEXCOORD0,
						out float2 rightTexCoord : TEXCOORD1)
			{
				const float PI = 3.14159;
				oPos = UnityObjectToClipPos (pos);
				texCoord = TRANSFORM_TEX (texCoord, _MainTex);
				leftTexCoord = texCoord + _leftSeparation * cos(fmod(_Time.x*_speed,PI)-PI/2);
				rightTexCoord = texCoord + _rightSeparation * cos(fmod(_Time.x*_speed,PI)-PI/2);
 
			}
			
			

			void frag(  float2 leftTexCoord : TEXCOORD0,
						float2 rightTexCoord : TEXCOORD1,
						out float4 color : COLOR,
						//Get the lerp value we exposed earlier
						uniform float _lerpValue)
			{
				float4 leftColor = tex2D(_MainTex, leftTexCoord);
				float4 rightColor = tex2D(_MainTex, rightTexCoord);
				color = lerp(leftColor, rightColor, _lerpValue);
			}
 

		ENDCG
		}
	}	
}