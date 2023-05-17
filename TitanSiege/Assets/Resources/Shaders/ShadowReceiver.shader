// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "M/ShadowReciever" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ShadowMap("Shadow Map", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		LOD 200
		
		Pass {
			CGPROGRAM
			#define LIGHTMAP_ON
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler _MainTex;
			half4 _MainTex_ST;
			sampler _ShadowMap;
			float4x4 _LocalToShadowMatrix;
			// half4 unity_LightmapST;
			// sampler2D unity_Lightmap;
			fixed4 _Color;

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			}; 
			
			struct v2f {
				float4  pos : SV_POSITION;
				float4  posLightSpace : TEXCOORD0;
				half2 uv : TEXCOORD1;
				#ifdef LIGHTMAP_ON
				half2 uvLM : TEXCOORD2;
				#endif
			};


			v2f vert (appdata v)
			{ 
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.posLightSpace = mul (_LocalToShadowMatrix, v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			#ifdef LIGHTMAP_ON 
				o.uvLM = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
			#endif
				return o;
			} 

 
			fixed4 frag (v2f i) : COLOR
			{
				float2 shadowCoord = half2(0.5, 0.5) * (i.posLightSpace.xy / i.posLightSpace.w) + half2(0.5,0.5);
				float4 shadow = tex2D(_ShadowMap, shadowCoord);

				fixed4 tex = tex2D (_MainTex, i.uv.xy);	

			#ifdef LIGHTMAP_ON
				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM));
				tex.rgb *= lm;	
			#else
				tex.rgb *= 2.0 * _Color;	
			#endif	

			tex.xyz = tex.xyz * (1.0 - shadow.a) + shadow.xyz * shadow.a;
			
			return tex;


			}

			ENDCG
		}
	} 
}
