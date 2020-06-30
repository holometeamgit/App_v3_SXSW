// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/DissolveVF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
        _SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0

        _BurnSize("Burn Size", Range(0.0, 1.0)) = 0.15
        _BurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
        _BurnColor("Burn Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 posWorld : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SliceGuide;
            float _SliceAmount;

            sampler2D _BurnRamp;
            fixed4 _BurnColor;
            float _BurnSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float cameraDist = length(i.posWorld.xyz - _WorldSpaceCameraPos.xyz);
                cameraDist = -cameraDist;//-floor(cameraDist * 10 + 0.5) * 0.1;

                //half posSin = 0.5 * sin( 0.5 * _Time.y * cameraDist) + 0.5;
			    //half pulseMultiplier = posSin * ( 1 - 0 ) + 0;

                float3 sliceGuide = tex2D(_SliceGuide, i.uv).rgb;
                sliceGuide = 1 - sliceGuide;
                float t = 5 * fmod(_Time.y * 0.5 + cameraDist, 5);

                _SliceAmount = 0.5 * sin(t) + 0.5;// * (1 - abs(_SinTime.w));
                half test = (_SliceAmount - sliceGuide.rgb);
                clip(test);

                col.a *= min(1, col.a / -cameraDist) * 0.75 * test;
                
                float3 emmission = tex2D(_BurnRamp, float2(test * (1 / _BurnSize), 0)) * _BurnColor;

                col.xyz += emmission;
                col.a += emmission * _BurnColor.a;

                return half4(col.xyz, col.a);
            }
            ENDCG
        }
    }
}
