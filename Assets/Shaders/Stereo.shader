Shader "Monash/Stereo"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
     	_SecondaryTex ("Secondary Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float4 vertex : SV_POSITION;
				float3 rawVert : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.rawVert = v.vertex;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _SecondaryTex;
			int _ScreenHeight;

			fixed4 frag (v2f i) : SV_Target
			{
				float y = i.rawVert.y * _ScreenHeight;

				fixed4 col = fixed4(0.0f, 0.0, 0.0, 1.0);

				if (fmod((int)y, 2) == 0)
				{
					col = tex2D(_MainTex, i.uv);
				}
				else
				{
					col = tex2D(_SecondaryTex, i.uv);
				}

				return col;
			}
			ENDCG
		}
	}
}
