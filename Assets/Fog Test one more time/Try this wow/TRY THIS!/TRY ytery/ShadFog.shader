Shader "Hidden/URPFog"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

    TEXTURE2D(_CameraDepthTexture);
    SAMPLER(sampler_CameraDepthTexture);

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);

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

    Varyings Vert(Attributes input)
    {
        Varyings output;
        output.positionHCS = TransformObjectToHClip(input.positionOS);
        output.uv = input.uv;
        return output;
    }

    float4 _FogColor;
    float _FogDensity;
    float _FogOffset;
    float _NearClipPlane;
    float _FarClipPlane;

    float4 Frag(Varyings input) : SV_Target
    {
        UNITY_SETUP_INSTANCE_ID(input);

        float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv).r;
        float linearDepth = LinearEyeDepth(depth, _NearClipPlane, _FarClipPlane);

        float viewDistance = linearDepth * _FarClipPlane;

        float fogFactor = (_FogDensity / sqrt(log(2.0))) * max(0.0f, viewDistance - _FogOffset);
        fogFactor = exp2(-fogFactor * fogFactor);

        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
        float4 fogOutput = lerp(_FogColor, col, saturate(fogFactor));

        return fogOutput;
    }
    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "URPFog"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
