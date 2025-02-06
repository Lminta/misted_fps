Shader "Custom/MistVolumetric"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MaxDistance("Max distance", float) = 100
        _StepSize("Step size", Range(0.1, 20)) = 1
        _DensityMultiplier("Density multiplier", Range(0, 1)) = 1
        _NoiseOffset("Noise offset", float) = 0
        
        _FogNoise("Fog noise", 3D) = "white" {}
        _NoiseTiling("Noise tiling", float) = 1
        _DensityThreshold("Density threshold", Range(0, 1)) = 0.1
        
        [HDR]_LightContribution("Light contribution", Color) = (1, 1, 1, 1)
        _LightScattering("Light scattering", Range(0, 1)) = 0.2
        
        _SunSize("Sun size", Range(0, 10)) = 1
        _SunIntensity("Sun intensity", float) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float4 _Color;
            float _MaxDistance;
            float _StepSize;
            float _DensityMultiplier;
            float _NoiseOffset;

            TEXTURE3D(_FogNoise);
            float _DensityThreshold;
            float _NoiseTiling;

            float4 _LightContribution;
            float _LightScattering;

            float _SunSize;
            float _SunIntensity;

            float henyey_greenstein(float angle, float scattering)
            {
                return (1.0 - angle * angle) /
                    (4.0 * PI * pow(1.0 + scattering * scattering - (2.0 * scattering) * angle, 1.5f));
            }

            float get_density(float3 worldPos)
            {
                float4 noise = _FogNoise.SampleLevel(sampler_TrilinearRepeat, worldPos * 0.01 * _NoiseTiling, 0);
                float density = dot(noise, noise);
                density = saturate(density - _DensityThreshold) * _DensityMultiplier;
                return density;
            }
            
            half4 frag(Varyings i) : SV_Target
            {
                float color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, i.texcoord);
                float depth = SampleSceneDepth(i.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(i.texcoord, depth, UNITY_MATRIX_I_VP);

                float3 entryPoint = _WorldSpaceCameraPos;
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLength = length(viewDir);
                float3 rayDir= normalize(viewDir);

                float2 pixelCoords = i.texcoord * _BlitTexture_TexelSize.zw;
                float distLimit = min(viewLength, _MaxDistance);
                float distTravelled = InterleavedGradientNoise(
                    pixelCoords, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _NoiseOffset;
                float transmittance = 1;
                float4 fogColor = _Color;

                while (distTravelled < distLimit)
                {
                    float3 rayPos = entryPoint + rayDir * distTravelled;
                    float density = get_density(rayPos);
                    if (density > 0)
                    {
                        Light mainLight = GetMainLight(TransformWorldToShadowCoord(rayPos));
                        float lightPhase = henyey_greenstein(dot(rayDir, mainLight.direction), _LightScattering);
                        float lightStrength = mainLight.shadowAttenuation * _StepSize * density;

                        // Apply normal light contribution
                        fogColor.rgb += mainLight.color.rgb * _LightContribution.rgb * lightPhase * lightStrength;

                        // Ensure the sun itself is visible
                        float size = 1 - _SunSize * 0.001;
                        float sunIntensity = max(_SunIntensity, 0);
                        float sunVisibility = smoothstep(size, 1.0, dot(rayDir, mainLight.direction)); 
                        fogColor.rgb += mainLight.color.rgb * _LightContribution.rgb * sunVisibility * sunIntensity;

                        transmittance *= exp(-density * _StepSize * (1.0 + distTravelled / _MaxDistance));
                    }
                    
                    distTravelled += _StepSize;
                }

                if (distTravelled >= _MaxDistance)
                {
                    transmittance = 0;
                }
                
                return lerp(color, fogColor, 1.0 - saturate(transmittance));
            }
            
            ENDHLSL
        }
    }
}
