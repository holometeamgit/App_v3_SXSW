Shader "HLM/Unlit/MlBeemOutput"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        [Toggle] _Blur("Blur", Float) = 1
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
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
                float4 _MainTex_ST;

                sampler2D _MaskTex;
                float4 _MaskTex_TexelSize;

                float _Blur;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    fixed4 mask = tex2D(_MaskTex, i.uv);

                    if (_Blur > 0)
                    {
                        fixed offset_x = _MaskTex_TexelSize.x;
                        fixed offset_y = _MaskTex_TexelSize.y;

                        fixed4 m11 = tex2D(_MaskTex, float2(i.uv.x + offset_x, i.uv.y));
                        fixed4 m12 = tex2D(_MaskTex, float2(i.uv.x, i.uv.y + offset_y));
                        fixed4 m13 = tex2D(_MaskTex, float2(i.uv.x - offset_x, i.uv.y));
                        fixed4 m14 = tex2D(_MaskTex, float2(i.uv.x + offset_x, i.uv.y - offset_y));

                        fixed4 m21 = tex2D(_MaskTex, float2(i.uv.x + offset_x, i.uv.y + offset_y));
                        fixed4 m22 = tex2D(_MaskTex, float2(i.uv.x - offset_x, i.uv.y - offset_y));
                        fixed4 m23 = tex2D(_MaskTex, float2(i.uv.x + offset_x, i.uv.y - offset_y));
                        fixed4 m24 = tex2D(_MaskTex, float2(i.uv.x - offset_x, i.uv.y + offset_y));

                        fixed4 m31 = tex2D(_MaskTex, float2(i.uv.x + 2.0 * offset_x, i.uv.y));
                        fixed4 m32 = tex2D(_MaskTex, float2(i.uv.x, i.uv.y + 2.0 * offset_y));
                        fixed4 m33 = tex2D(_MaskTex, float2(i.uv.x - 2.0 * offset_x, i.uv.y));
                        fixed4 m34 = tex2D(_MaskTex, float2(i.uv.x + 2.0 * offset_x, i.uv.y - 2.0 * offset_y));

                        fixed4 m41 = tex2D(_MaskTex, float2(i.uv.x + 2.0 * offset_x, i.uv.y + 2.0 * offset_y));
                        fixed4 m42 = tex2D(_MaskTex, float2(i.uv.x - 2.0 * offset_x, i.uv.y - 2.0 * offset_y));
                        fixed4 m43 = tex2D(_MaskTex, float2(i.uv.x + 2.0 * offset_x, i.uv.y - 2.0 * offset_y));
                        fixed4 m44 = tex2D(_MaskTex, float2(i.uv.x - 2.0 * offset_x, i.uv.y + 2.0 * offset_y));

                        mask = (mask +
                            (m11 + m12 + m13 + m14) +
                            (m21 + m22 + m23 + m24) +
                            (m31 + m32 + m33 + m34) +
                            (m41 + m42 + m43 + m44)) / 8.0;
                    }

                    col = col * (1 - mask.g) + mask * mask.g;

                    //col = mask;

                    return col;
                }
                ENDCG
            }
        }
}
