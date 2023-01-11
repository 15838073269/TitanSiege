// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "M/ShadowMap" {
	Properties {
		
	}
	SubShader {
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		LOD 200
		
		Pass { 
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"


 
			struct appdata {
				float4 vertex : POSITION;

			};
			
			struct v2f {
				float4  pos : SV_POSITION;
				//float4  ssPos : TEXCOORD1;
			};


			v2f vert (appdata v)
			{ 
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				//o.ssPos = o.pos;
				return o;
			} 

 
			fixed4 frag (v2f i) : COLOR
			{
				//return i.ssPos.z / i.ssPos.w;
				return fixed4(0, 0, 0, 0.5);
			}

			ENDCG
		}
	} 
}
