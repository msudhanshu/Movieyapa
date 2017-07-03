// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/Effect/Ripple_PerVertex" {
   Properties {
        _Ambient("Ambient Color",Color) = (0.3,0.3,0.3,1)
        _MainTex ("RGBA Texture Image", 2D) = "white" {} 
		_Rate ("Rate", Range(0,1)) = 0.5
		centerx ("CenterX", Range(0,1)) = 0.5
		centery ("CenterY", Range(0,1) ) = 0.5
		
   }
   SubShader {
      Pass {    
       Tags { "LightMode" = "Vertex" }//otherwise no light related values will be filled
         Cull Off // since the front is partially transparent, 
            // we shouldn't cull the back
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 #include "UnityCG.cginc"
 
 		uniform fixed4 _Ambient;
         uniform sampler2D _MainTex;    
         uniform sampler2D _MainTex_ST;  
 		uniform float _Rate;
 		uniform float centerx;
		uniform float centery;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
            fixed4 color: COLOR;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 		
	       	input.vertex.y += centerx*sin(dot(input.vertex.xz,input.vertex.xz));
 			
 			input.normal.x = 2. * 0.3 * input.vertex.x * cos(dot(input.vertex.xz,input.vertex.xz));
  			input.normal.z = 2. * 0.3 * input.vertex.z * cos(dot(input.vertex.xz,input.vertex.xz));
 			input.normal.y = 1;
 			
 			fixed3 normalDirction = normalize(input.normal);
  
  
        // per vertex light calc
               fixed3 lightDirection;
               fixed attenuation;
              fixed3 objectSpaceLightPos = mul(unity_LightPosition[0],UNITY_MATRIX_IT_MV).xyz; //ObjectSpaceLightPos[0].xyz;
              
               
                 // add diffuse
               if(unity_LightPosition[0].w == 0.0)//directional light
               {
                  attenuation = 2;
                  lightDirection = normalize(objectSpaceLightPos);
               }
               else// point or spot light
               {
                  lightDirection = normalize( objectSpaceLightPos - input.vertex.xyz);
                  attenuation = 1.0/(length( objectSpaceLightPos - input.vertex.xyz)) * 0.5;
               }
             
               
               // unity_LightColor[0].xyz _... not ... ModelLightColor[0].xyz
               fixed3 diffuseLight =  unity_LightColor[0].xyz * max(dot(normalDirction,lightDirection),0);
               // combine the lights (diffuse + ambient)
               output.color.xyz = diffuseLight * attenuation + _Ambient.xyz;
               output.color.a = 1;
               
            output.tex = input.texcoord;
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float4 textureColor = tex2D(_MainTex, float2(input.tex));  
            return input.color*textureColor;
         }
 
         ENDCG
      }
   }
 
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Unlit/Transparent Cutout"
}