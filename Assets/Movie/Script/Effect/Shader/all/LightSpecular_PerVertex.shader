// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/LightSpecular_PerVertex" {
      Properties {
        _MainTex ("Texture 1", 2D) = "white" {}
        _Ambient("Ambient Color",Color) = (0.3,0.3,0.3,1)
        _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
        _shininess ("Shininess",Range(0.5,20.0)) = 2
        _Ks ("Ks",Range(0.0,1.0)) = 0.5
        
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
           uniform fixed4 _Ambient;
           half4 _MainTex_ST;
   		   uniform fixed _shininess;
   		   uniform float4 _SpecColor; 
   		   uniform fixed _Ks;
           struct v2f{
               fixed4 position: POSITION;
               half2 uv_MainTex:TEXCOORD0;
               fixed4 color: COLOR;
           };
   
           v2f vert (appdata_base ab) {
               v2f o;
    
               o.position = UnityObjectToClipPos(ab.vertex);
               o.uv_MainTex = TRANSFORM_TEX(ab.texcoord.xy , _MainTex);
    
    			//add ambient light
    		fixed3 ambientLight = fixed3(UNITY_LIGHTMODEL_AMBIENT) *  _Ambient.xyz;
               
               // per vertex light calc
              fixed3 lightDirection;
              fixed attenuation;
              //_WorldSpaceLightPos0  != unity_LightPosition[0]
              fixed3 objectSpaceLightPos = mul(unity_LightPosition[0] ,UNITY_MATRIX_IT_MV).xyz; //ObjectSpaceLightPos[0].xyz;   
              fixed3 normalDirection = normalize(ab.normal);
       
               // add diffuse
               //diffuse = Kd x lightColor x max(N · L, 0)
               if(unity_LightPosition[0].w == 0.0)//directional light
               {
                  attenuation = 2;
                  lightDirection = normalize(objectSpaceLightPos);
               }
               else// point or spot light
               {
                  lightDirection = normalize( objectSpaceLightPos - ab.vertex.xyz);
                  attenuation = 1.0/(length( objectSpaceLightPos - ab.vertex.xyz)) * 0.5;
               }
               fixed NdotL = dot(normalDirection,lightDirection);
               // unity_LightColor[0].xyz _... not ... ModelLightColor[0].xyz
               fixed3 diffuseLight = attenuation * unity_LightColor[0].xyz * max(NdotL,0);
              
               //add specular
                //specular = Ks x lightColor x facing x (max(N · H, 0))^shininess
                //facing is 1 if N · L is greater than 0, and 0 otherwise.
                //V is the normalized vector toward the viewpoint,
				//L is the normalized vector toward the light source,
				//H is the normalized vector that is halfway between V and L,
				
				float3 viewDirection = normalize(float3(
               float4(_WorldSpaceCameraPos, 1.0) 
               - mul(unity_ObjectToWorld, ab.vertex)));
               
               fixed facing = NdotL>0?1:0;
               fixed3 H = lightDirection;
               fixed3 specularLight = facing * _Ks * _SpecColor.xyz * 
              			 pow(max( dot
              			 //(normalDirection,H),
              			 (reflect(-lightDirection, normalDirection), viewDirection),
              			 0),
              			 _shininess );
                
                
                  // combine the lights (specular + diffuse + ambient)
               o.color.xyz =  specularLight  + diffuseLight  + ambientLight;
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