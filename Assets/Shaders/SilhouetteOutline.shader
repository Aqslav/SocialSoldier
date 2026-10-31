Shader "Custom/SilhouetteOutline"
{
    Properties
    {
        _OutlineColor ("Color", Color) = (1,1,1,1)
        _OutlineWidth ("Width (pixels)", Float) = 3
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        CBUFFER_START(UnityPerMaterial)
            half4 _OutlineColor;
            float _OutlineWidth;
        CBUFFER_END
        ENDHLSL

        // Pass 0: draw the object as a flat white shape into the mask
        Pass
        {
            Name "Mask"
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings   { float4 positionCS : SV_POSITION; };
            Varyings vert(Attributes i)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                return o;
            }
            half4 frag(Varyings i) : SV_Target { return 1; }
            ENDHLSL
        }

        // Pass 1: fullscreen, draw a line around the mask
        Pass
        {
            Name "Composite"
            Cull Off ZWrite Off ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.texcoord;
                float center = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv).r;
                if (center > 0.5) return 0;   // inside the object: draw nothing

                float2 px = _BlitTexture_TexelSize.xy * _OutlineWidth;
                float hit = 0;
                for (int r = 1; r <= 3; r++)
                {
                    for (int k = 0; k < 12; k++)
                    {
                        float a = k * (6.2831853 / 12.0);
                        float2 off = float2(cos(a), sin(a)) * px * (r / 3.0);
                        hit = max(hit, SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv + off).r);
                    }
                }
                return half4(_OutlineColor.rgb, _OutlineColor.a * step(0.5, hit));
            }
            ENDHLSL
        }
    }
}