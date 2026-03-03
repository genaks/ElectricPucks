Shader "Custom/ElectricCurrent_URP"
{
    Properties
    {
        _Color ("Glow Color", Color) = (0.3, 0.9, 1.0, 1.0)
        _CoreColor ("Core Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Intensity ("Intensity", Range(0.0, 10.0)) = 5.0
        _GlowWidth ("Glow Width", Range(0.001, 0.5)) = 0.08
        _CoreWidth ("Core Width", Range(0.001, 0.1)) = 0.008
        _ZigzagFrequency ("Zigzag Frequency", Range(1.0, 30.0)) = 8.0
        _ZigzagAmplitude ("Zigzag Amplitude", Range(0.0, 0.5)) = 0.15
        _AnimSpeed ("Animation Speed", Range(0.0, 20.0)) = 6.0
        _Flickering ("Flickering", Range(0.0, 1.0)) = 0.1
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

        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ElectricCurrent"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _CoreColor;
                float _Intensity;
                float _GlowWidth;
                float _CoreWidth;
                float _ZigzagFrequency;
                float _ZigzagAmplitude;
                float _AnimSpeed;
                float _Flickering;
            CBUFFER_END

            float hash(float p)
            {
                return frac(sin(p * 127.1) * 43758.5453);
            }

            // Returns the y value of the zigzag path at a given x
            // This is a true triangle wave: goes linearly up then linearly down
            float zigzagPath(float x)
            {
                float t = x * _ZigzagFrequency;
                // frac(t) gives 0->1 sawtooth
                // abs(frac - 0.5) * 2 gives 0->1->0 triangle
                // * 2 - 1 centres it at 0, ranging -1 to 1
                return (abs(frac(t) - 0.5) * 2.0) * 2.0 - 1.0;
            }

            // SDF: shortest distance from point p to the zigzag path
            // We compute this by checking distance to the line segment
            // within the current zigzag cell
            float distToZigzag(float2 p)
            {
                float freq = _ZigzagFrequency;
                float amp = _ZigzagAmplitude;

                float t = p.x * freq;
                float cell = floor(t);
                float f = frac(t);

                // 0 for even cells, 1 for odd cells - no ternary needed
                float isOdd = step(0.5, frac(cell * 0.5));

                // Even cell: -amp to +amp, Odd cell: +amp to -amp
                float y0 = lerp(-amp, amp, isOdd);
                float y1 = lerp(amp, -amp, isOdd);

                float2 a = float2(0.0, y0);
                float2 b = float2(1.0, y1);
                float2 q = float2(f, p.y);

                float2 ab = b - a;
                float2 aq = q - a;
                float s = saturate(dot(aq, ab) / dot(ab, ab));
                return length(aq - ab * s) / freq;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Remap uv.y to -0.5 .. 0.5
                float2 uv = float2(IN.uv.x, IN.uv.y - 0.5);
                float t = _Time.y * _AnimSpeed;

                // Animate by offsetting x
                uv.x += t * (1.0 / _ZigzagFrequency);

                // Flicker
                float flicker = 1.0 - _Flickering * hash(floor(t * 6.0));

                // Distance to zigzag path
                float dist = distToZigzag(uv);

                // Core: sharp bright line
                float core = 1.0 - smoothstep(0.0, _CoreWidth, dist);

                // Glow: soft halo
                float glow = 1.0 - smoothstep(0.0, _GlowWidth, dist);
                glow = pow(glow, 2.0);

                // Combine
                float3 col = _CoreColor.rgb * _Intensity * 2.0 * core
                    + _Color.rgb * _Intensity * glow;
                float alpha = saturate(core * 4.0 + glow) * _Color.a * flicker;

                return half4(col * flicker, alpha);
            }
            ENDHLSL
        }
    }
}