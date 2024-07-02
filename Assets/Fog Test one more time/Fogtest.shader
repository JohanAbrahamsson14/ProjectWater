Shader "Custom/FogURP"
{
    Properties
    {
        _MainTex("Base Map", 2D) = "white" {}
        _FogColor("Fog Color", Color) = (1,1,1,1)
        _FogStart("Fog Start Distance", Float) = 0.0
        _FogEnd("Fog End Distance", Float) = 100.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

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
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _FogColor;
            float _FogStart;
            float _FogEnd;

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                output.positionWS = TransformObjectToWorld(input.positionOS);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float3 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;

                // Calculate the distance from the camera to the fragment
                float3 worldPos = input.positionWS;
                float distance = length(worldPos - _WorldSpaceCameraPos);
                
                // Calculate fog factor based on linear distance
                float fogFactor = saturate((distance - _FogStart) / (_FogEnd - _FogStart));

                // Blend the fog color with the base color
                float3 foggedColor = lerp(baseColor, _FogColor.rgb, fogFactor);

                return half4(foggedColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
