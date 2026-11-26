Shader "Horror/Visibility"
{
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "HorrorVisibility"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 4.5

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            int _HVSourceCount;
            float4 _HVPositions[32];
            float4 _HVSettings[32];
            float _HVDarkness;
            float _MinBrightness;

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;

                half4 color = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv
                );

                float depth = SampleSceneDepth(uv);

                #if UNITY_REVERSED_Z
                    if (depth <= 0.00001)
                        return half4(
                            color.rgb * (1.0 - _HVDarkness),
                            color.a
                        );
                #else
                    if (depth >= 0.99999)
                        return half4(
                            color.rgb * (1.0 - _HVDarkness),
                            color.a
                        );

                    depth = lerp(
                        UNITY_NEAR_CLIP_VALUE,
                        1.0,
                        depth
                    );
                #endif

                float3 worldPosition =
                    ComputeWorldSpacePosition(
                        uv,
                        depth,
                        UNITY_MATRIX_I_VP
                    );

                float visibility = 0.0;

                for (int i = 0; i < _HVSourceCount; i++)
                {
                    float3 sourcePosition = _HVPositions[i].xyz;
                    float outerRadius = _HVPositions[i].w;
                    float innerRadius = _HVSettings[i].x;
                    float strength = _HVSettings[i].y;

                    float dist = distance(
                        worldPosition,
                        sourcePosition
                    );

                    float mask = 1.0 - smoothstep(
                        innerRadius,
                        outerRadius,
                        dist
                    );

                    visibility = max(
                            max(
                            visibility,
                            mask * strength),
                        _MinBrightness);
                }

                float brightness = lerp(
                    1.0 - _HVDarkness,
                    1.0,
                    saturate(visibility)
                );

                
                
                color.rgb *= brightness;

                return color;
            }

            ENDHLSL
        }
    }
}
