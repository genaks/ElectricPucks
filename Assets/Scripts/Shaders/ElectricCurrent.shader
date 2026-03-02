Shader "Custom/ElectricCurrent_URP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (0.3, 0.6, 1.0, 1.0)
        _CoreColor ("Core Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Intensity ("Intensity", Range(0.0, 10.0)) = 2.0
        _GlowWidth ("Glow Width", Range(0.0, 1.0)) = 0.4
        _CoreWidth ("Core Width", Range(0.0, 0.5)) = 0.08
        _NoiseScale ("Noise Scale", Range(1.0, 50.0)) = 10.0
        _NoiseSpeed ("Noise Speed", Range(0.0, 20.0)) = 8.0
        _Distortion ("Distortion Amount", Range(0.0, 1.0)) = 0.6
        _Flickering ("Flickering", Range(0.0, 1.0)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha One  // Additive blending for glow
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ElectricCurrent"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _CoreColor;
                float _Intensity;
                float _GlowWidth;
                float _CoreWidth;
                float _NoiseScale;
                float _NoiseSpeed;
                float _Distortion;
                float _Flickering;
            CBUFFER_END

            float hash(float2 p)
            {
                p = frac(p * float2(234.34, 435.345));
                p += dot(p, p + 34.23);
                return frac(p.x * p.y);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);

                return lerp(
                    lerp(hash(i),              hash(i + float2(1,0)), f.x),
                    lerp(hash(i + float2(0,1)), hash(i + float2(1,1)), f.x),
                    f.y
                );
            }

            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                for (int i = 0; i < 4; i++)
                {
                    value += amplitude * noise(p);
                    p *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float across = abs(uv.y - 0.5) * 2.0;
                float along = uv.x;

                float t = _Time.y * _NoiseSpeed;
                float zz1 = abs(frac(along * _NoiseScale       + t       ) - 0.5) * 2.0; // base zigzag
                float zz2 = abs(frac(along * _NoiseScale * 2.3 + t * 1.7 ) - 0.5) * 2.0; // higher freq layer
                float zz3 = abs(frac(along * _NoiseScale * 5.1 + t * 0.9 ) - 0.5) * 2.0; // detail layer

                float zigzag = (zz1 * 0.5 + zz2 * 0.3 + zz3 * 0.2) - 0.5; // combine & centre around 0

                float distortedAcross = across + zigzag * _Distortion * (1.0 - across * 0.5);
                float flicker = 1.0 - _Flickering * noise(float2(t * 3.0, 0.5));

                float core = 1.0 - smoothstep(0.0, _CoreWidth, distortedAcross);
                float glow = pow(1.0 - smoothstep(0.0, _GlowWidth, distortedAcross), 2.0);

                float2 tendrilUV = float2(along * _NoiseScale * 2.0 + t * 1.5, uv.y * 5.0);
                float tendrils = pow(max(0, fbm(tendrilUV) - 0.5) * 2.0, 3.0);
                tendrils *= (1.0 - across);

                float3 finalColor =
                    _Color.rgb    * _Intensity       * glow    * flicker +
                    _CoreColor.rgb * _Intensity * 2.0 * core    * flicker +
                    _Color.rgb    * _Intensity * 0.5 * tendrils * flicker;

                float alpha = saturate(glow + core * 2.0) * _Color.a * flicker;

                return half4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}