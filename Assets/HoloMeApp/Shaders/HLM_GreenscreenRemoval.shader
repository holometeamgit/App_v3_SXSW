Shader "HLM/Unlit/GreenscreenRemoval"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        [MaterialToggle] _On("On", Float) = 1.0
        
        [MaterialToggle] _ShowMatte("Matte", Float) = 1.0
        
        _H1("H1", Float) =  60
        _H2("H2", Float) = 120
        _S ("S",  Float) = 0.22
        _V ("V",  Float) = 0.21
        
        _smallPolyhedron1("sp1", Float) = 0.2
        _smallPolyhedron2("sp2", Float) = 0.3
        _mediumPolyhedron("mp",  Float) = 0.34
        _largePolyhedron ("lp",  Float) = 2.0
        
        _maskContrastL("maskContrastL", Float) = 0.01
        _maskContrastM("maskContrastM", Float) = 1.0
        _maskContrastS("maskContrastS", Float) = 1.0
        _maskContrast ("maskContrast",  Float) = 0.75


        [MaterialToggle] _UseBlendTex("UseBlendTex", Float) = 1.0
        _BlendTex("BlendTexture", 2D) = "white" {}

        [MaterialToggle] _DespillAndReflectionRemove("Despill And Reflection Remove", Float) = 1.0

        _rH1("rH1", Float) =  30
        _rH2("rH2", Float) = 150
        _rS ("rS",  Float) = 0.1
        _rV ("rV",  Float) = 0.1

        _srFactor("srFactor", Float) =  1.25
        _sgFactor("sgFactor", Float) =  0.97

        [Toggle(USE_AMBIENT_LIGHTING)] _UseAmbientLighting("Use ambient lighting", Float) = 1
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "LightMode" = "ForwardBase"}
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_AMBIENT_LIGHTING

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

			#define M_PI 3.1415926535897932384626433832795

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 diff : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BlendTex;
            float4 _BlendTex_ST;
            float _UseBlendTex;
            
            float _On;
            float _ShowMatte;
            
            float _H1;
            float _H2;
            float _S;
            float _V;
            float _Distance;
            
            float _smallPolyhedron1;
            float _smallPolyhedron2;
            float _mediumPolyhedron;
            float _largePolyhedron;
            float _maskContrastL;
            float _maskContrastM;
            float _maskContrastS;
            float _maskContrast;
            
            uniform float Epsilon = 1e-10;
            float4 _MainTex_TexelSize;
            
            float _DespillAndReflectionRemove;

            float _rH1; 
            float _rH2; 
            float _rS;
            float _rV;

            float _srFactor;
            float _sgFactor;

            float3 RGBtoHCV(float3 RGB) {
                float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
                float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
                float C = Q.x - min(Q.w, Q.y);
                float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
                return float3(H, C, Q.x);
            }
            
            float3 RGBtoHSV(float3 RGB) {
                float3 HCV = RGBtoHCV(RGB);
                float S = HCV.y / (HCV.z + Epsilon);
                return float3(HCV.x, S, HCV.z);
            }
            
             float3 hsv2rgb(float3 c)
             {
              c = float3(c.x, clamp(c.yz, 0.0, 1.0));
              float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.diff = _LightColor0;
                return o;
            }
            
            float hsvDistance(float3 hsv, float ih1, float ih2, float is, float iv)
            {
                float h = hsv.r * 255;
                float s = hsv.g;
                float v = hsv.b;
                
                float dh1 = min(abs(h-ih1), 360-abs(h-ih1)) / 180.0;
                float dh2 = min(abs(h-ih2), 360-abs(h-ih2)) / 180.0;
                
                float dh = min(dh1, dh2);
                
                float ds = abs(s-is);
                float dv = abs(v-iv);
                
                float distance = sqrt(dh*dh  + ds*ds + dv*dv);
                return distance;
            }
            
            float3 adjustContrastCurve(float3 color, float contrast) {
                return pow(abs(color * 2 - 1), 1 / max(contrast, 0.0001)) * sign(color - 0.5) + 0.5;
            }
            
            bool isHSVgood(float3 hsv, float ih1, float ih2, float is, float iv) {
                return hsv.r * 255 >= ih1 && hsv.r * 255 <= ih2 && hsv.g >= is && hsv.b >= iv;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                if (_On == 0)
                {
                    return col;
                }
                
                float3 hsv = RGBtoHSV(col.rgb);
                
                fixed4 up    = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y));
                fixed4 down  = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y));
                fixed4 left  = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, 0));
                fixed4 right = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0));
                
                float3 hsv_up    = RGBtoHSV(up.rgb);
                float3 hsv_down  = RGBtoHSV(down.rgb);
                float3 hsv_left  = RGBtoHSV(left.rgb);
                float3 hsv_right = RGBtoHSV(right.rgb);
                float distance = 0;
                
                if (isHSVgood(hsv, _H1, _H2, _S, _V)) {
                    distance  = hsvDistance(hsv, _H1, _H2, _S, _V);
                    
                    float distanceL = hsvDistance(hsv_left, _H1, _H2, _S, _V);
                    float distanceR = hsvDistance(hsv_right, _H1, _H2, _S, _V);
                    float distanceU = hsvDistance(hsv_up, _H1, _H2, _S, _V);
                    float distanceD = hsvDistance(hsv_down, _H1, _H2, _S, _V);
                    
                    float d = 1  - distance;
                    
                    col.a = d * _maskContrast;
                    if (distance < _smallPolyhedron1)
                    {
                        if (distanceL > _smallPolyhedron1 || distanceR > _smallPolyhedron1 || distanceU > _smallPolyhedron1 || distanceD > _smallPolyhedron1) {
                        }
                        else {
                            col.a = d * _maskContrastS;
                        }
                    } else if (distance < _smallPolyhedron2) {
                        if (distanceL > _smallPolyhedron2 || distanceR > _smallPolyhedron2 || distanceU > _smallPolyhedron2 || distanceD > _smallPolyhedron2) {
                        } else {
                            col.a = d * _maskContrastS;
                        }
                    } else if (distance < _mediumPolyhedron) {
                        if (distanceL > _mediumPolyhedron || distanceR > _mediumPolyhedron || distanceU > _mediumPolyhedron || distanceD > _mediumPolyhedron) {
                            col.a = d * _maskContrastM;
                        }
                    } else if (distance < _largePolyhedron) {
                        col.a = d * _maskContrastL;
                    }
                }
                
                if (_ShowMatte != 0) {
                    return fixed4(col.a, col.a, col.a, 1.0);
                }
                
                if (_DespillAndReflectionRemove > 0.0) {
                    distance = min(1.0 - hsvDistance(hsv, _rH1, _rH2, _rS, _rV), 1.0);
                    if (isHSVgood(hsv, _rH1, _rH2, _rS, _rV)) {
                        hsv.g = min(hsv.g * distance * _srFactor, hsv.g);
                        col.rgb = hsv2rgb(hsv);                    
                        col.rgb = float3(col.r, col.g * _sgFactor, col.b);
                    }
                }

                //  TODO - add darkness on the edges and blur it a bit

#ifdef USE_AMBIENT_LIGHTING
				   i.diff.rgb = clamp(i.diff.rgb, float3(0.25, 0.25, 0.25), float3(1.75, 1.75, 1.75));
				   col.rgb *= i.diff;
#endif                   
                   fixed4 blendTextureColour = tex2D(_BlendTex, i.uv);
                   
                   if (_UseBlendTex == 0) {
                        blendTextureColour.a = 1;
                   }

                return fixed4(col.rgb, col.a * blendTextureColour.a);
            }
            ENDCG
        }
    }
}
