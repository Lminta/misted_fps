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
        
        _LightScattering("Light scattering", Range(0, 1)) = 0.9
        _AdditionalLightScatteringMultiplier("Addition light scattering multiplier", Range(0, 100)) = 10
        
        _SunSize("Sun size", Range(0, 10)) = 1
        _SunIntensity("Sun intensity", float) = 1
        
        _MaxAdditionalLightSources("Max additional light sources", Integer) = 3
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

            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Macros.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Random.hlsl"

            #pragma multi_compile _ _FORWARD_PLUS

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            float4 _Color;
            float _MaxDistance;
            float _StepSize;
            float _DensityMultiplier;
            float _NoiseOffset;

            TEXTURE3D(_FogNoise);
            float _DensityThreshold;
            float _NoiseTiling;
            
            float _LightScattering;
            float _AdditionalLightScatteringMultiplier;

            float _SunSize;
            float _SunIntensity;

            int _MaxAdditionalLightSources;
            
            uint _CustomAdditionalLightsCount;
            float _Anisotropies[MAX_VISIBLE_LIGHTS + 1];
            float _Scatterings[MAX_VISIBLE_LIGHTS + 1];
            float _RadiiSq[MAX_VISIBLE_LIGHTS];


            float henyey_greenstein(float angle, float scattering)
            {
                return (1.0 - angle * angle) /
                    (4.0 * PI * pow(abs(1.0 + scattering * scattering - (2.0 * scattering) * angle), 1.5f));
            }

            float get_density(float3 worldPos)
            {
                float4 noise = _FogNoise.SampleLevel(sampler_TrilinearRepeat, worldPos * 0.01 * _NoiseTiling, 0);
                float density = dot(noise, noise);
                density = saturate(density - _DensityThreshold) * _DensityMultiplier;
                return density;
            }

            float3 main_light_contribution(Light light, float3 rayDir, float density)
            {
                float lightPhase = henyey_greenstein(dot(rayDir, light.direction), _LightScattering);
                float lightStrength = light.shadowAttenuation * _StepSize * density;
                return light.color.rgb * lightPhase * lightStrength;
            }
            
            float3 additional_light_contribution(float2 uv, float3 rayPos, float3 rayDir, float density)
            {
#if _ADDITIONAL_LIGHTS_CONTRIBUTION_DISABLED
                return float3(0.0, 0.0, 0.0);
#endif
#if _FORWARD_PLUS
                // Forward+ rendering path needs this data before the light loop
                InputData inputData = (InputData)0;
                inputData.normalizedScreenSpaceUV = uv;
                inputData.positionWS = rayPos;
#endif
                float3 additionalLightsColor = float3(0.0, 0.0, 0.0);   
                
                
                LIGHT_LOOP_BEGIN(min(_CustomAdditionalLightsCount, _MaxAdditionalLightSources))
                
                    Light additionalLight = GetAdditionalPerObjectLight(lightIndex, rayPos);
                    
                    float lightPhase = henyey_greenstein(dot(rayDir, additionalLight.direction), _LightScattering);

                    float additionalLightScattering = _LightScattering * _AdditionalLightScatteringMultiplier;
                
                    additionalLightsColor +=
                        additionalLight.color.rgb * lightPhase * additionalLight.shadowAttenuation * _StepSize *
                            density  * additionalLightScattering * additionalLight.distanceAttenuation;
                LIGHT_LOOP_END

                return additionalLightsColor;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
                float depth = SampleSceneDepth(IN.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);
                float3 entryPoint = _WorldSpaceCameraPos;
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLength = length(viewDir);
                float3 rayDir= normalize(viewDir);
                
                float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw;
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
                        float4 shadowPos = TransformWorldToShadowCoord(rayPos);
                        Light mainLight = GetMainLight(shadowPos);
                        fogColor.rgb += main_light_contribution(mainLight, rayDir, density);
                        
                        fogColor.rgb += additional_light_contribution(IN.texcoord, rayPos, rayDir, density);
                
                        transmittance *= exp(-density * _StepSize * (1.0 + distTravelled / _MaxDistance));
                    }
                    
                    distTravelled += _StepSize;
                }
                
                // Ensure the sun itself is visible
                if (distTravelled >= _MaxDistance)
                {
                    Light mainLight = GetMainLight();
                    float size = 1 - _SunSize * 0.001;
                    float sunIntensity = max(_SunIntensity, 0);
                    float sunVisibility = smoothstep(size, 1.0, dot(rayDir, mainLight.direction)); 
                    fogColor.rgb += mainLight.color.rgb * sunVisibility * sunIntensity;
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
