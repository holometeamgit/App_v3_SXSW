Shader "Custom/MaskShader"
{
	Properties
	{
		_MainTex("Texture (RGB)", 2D) = "white" {}
		_MaskTex("Mask (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Threshhold("Threshhold", Float) = 0.3
	}
		SubShader
		{
			 Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			AlphaTest Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#pragma multi_compile_fog

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
				sampler2D _MaskTex;
				float _Threshhold;
				fixed4 _Color;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, IN.uv) * (tex2D(_MaskTex, IN.uv)) + (1 - tex2D(_MaskTex, IN.uv)) * _Color;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
}