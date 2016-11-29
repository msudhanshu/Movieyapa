Shader "MyShader/Effect/ManyRippleDots" {
   Properties {
        _Ambient("Ambient Color",Color) = (0.3,0.3,0.3,1)
        _MainTex ("RGBA Texture Image", 2D) = "white" {} 
		_Rate ("Rate", Range(-10,10)) = 1
		_Height ("Height", Range(0,1)) = 0.5
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
        uniform float4 _MainTex_ST;  
 		uniform float _Rate;
 		uniform float centerx;
		uniform float centery;
		uniform float _Height;
		
         struct vertexInput {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float2 tex : TEXCOORD0;
            float4 vertex : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 			output.vertex = input.vertex;
            // pass the normal further to fragment shader if u want
            //output.tex = input.texcoord.xy;
            output.tex  = TRANSFORM_TEX (input.texcoord, _MainTex);
            
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	float phi = _Time.w*_Rate;
 			float2 texcord = input.tex;
          	//input.vertex.y += _Height*sin(phi +dot(input.vertex.xz,input.vertex.xz));
 			float3 normal=1;
 			//2. * 0.3 *
 		//	normal.x = input.vertex.x * _Height * cos(phi + dot(input.vertex.xz,input.vertex.xz));
  		//	normal.z = input.vertex.z  * _Height * cos(phi + dot(input.vertex.xz,input.vertex.xz));
  			float2 texcord_wrt_center = texcord - float2(centerx,centery);
  			normal.x = _Height * cos(phi*dot(texcord_wrt_center,texcord_wrt_center));
  			normal.z =  normal.x;
  			
 		//	normal.y = 1;
 			
 			fixed3 normalDirction = normalize(normal);
  
  
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
               float4 color=1;
               color.xyz = diffuseLight * attenuation + _Ambient.xyz;
               color.a = 1;
               
            float4 textureColor = tex2D(_MainTex, texcord);  
            return color*textureColor;
         }
 
         ENDCG
      }
   }
 
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Unlit/Transparent Cutout"
}