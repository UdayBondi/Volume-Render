﻿Shader "Unlit/Cube_shader"
{
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }


		Pass
		{   
            
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
          
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				//o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = float4(0,0,0,0);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = fixed4(0.0,0.0,0.0,0.0f);

				return col;
			}
			ENDCG
		}
	}
}
