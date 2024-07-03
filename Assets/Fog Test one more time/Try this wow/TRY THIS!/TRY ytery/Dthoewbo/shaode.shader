Shader "Hidden/CustomURPFog"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _FogColor("Fog Color", Color) = (1, 1, 1, 1)
        _FogDensity("Fog Density", Float) = 0.5
        _FogOffset("Fog Offset", Float) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            float4 _FogColor;
            float _FogDensity;
            float _FogOffset;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.position = TransformObjectToHClip(input.vertex);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv).r;
                float linearDepth = LinearEyeDepth(depth, _ZBufferParams);
                float viewDistance = linearDepth * _ProjectionParams.z;
                float fogFactor = (_FogDensity / sqrt(log(2))) * max(0.0f, viewDistance - _FogOffset);
                fogFactor = exp2(-fogFactor * fogFactor);
                return lerp(_FogColor, color, saturate(fogFactor));
            }
            ENDHLSL
        }
    }
}
