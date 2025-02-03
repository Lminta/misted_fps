Shader "Custom/CloudFogParticle"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Scale("Noise Scale", Range(0.1, 10)) = 2
        _Speed("Animation Speed", Range(0, 2)) = 0.3
        _Density("Density", Range(0, 2)) = 1.2
        _Softness("Softness", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _Scale;
            float _Speed;
            float _Density;
            float _Softness;

            float random(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 uv) {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(random(i + float2(0,0)), random(i + float2(1,0)), f.x),
                            lerp(random(i + float2(0,1)), random(i + float2(1,1)), f.x), f.y);
            }

            float fbm(float2 uv) {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;

                for (int i = 0; i < 5; i++) {
                    value += amplitude * noise(uv * frequency);
                    frequency *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {

                // Use the random value passed from TEXCOORD1 to modify the UV and animation
                float2 uv = i.uv * _Scale;
                float2 animatedUV = uv + _Time.y * _Speed * float2(0.1, 0.2);

                float cloudPattern = fbm(animatedUV);
                cloudPattern = smoothstep(0.3, 0.7, cloudPattern);

                float2 centerVector = i.uv - 0.5;
                float radialMask = 1.0 - pow(length(centerVector) * 1.8, 2.0);
                radialMask = smoothstep(0.0, 1.0, radialMask);

                float finalAlpha = saturate(cloudPattern * _Density * radialMask);
                finalAlpha = smoothstep(_Softness * 0.3, 1.0, finalAlpha);

                // Final color output with random variation
                return half4(_Color.rgb, finalAlpha);
            }
            ENDHLSL
        }
    }
}
