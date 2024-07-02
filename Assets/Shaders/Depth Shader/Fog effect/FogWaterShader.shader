Shader "Custom/AdvancedWaterFogShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DepthColor ("Depth Color", Color) = (0, 0.2, 0.5, 1)
        _SurfaceColor ("Surface Color", Color) = (0.2, 0.8, 1, 1)
        _FogStart ("Fog Start Distance", Float) = 0.0
        _FogEnd ("Fog End Distance", Float) = 50.0
        _InterpolationFactor ("Interpolation Factor", Range(0, 1)) = 0.5
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _DepthColor;
            float4 _SurfaceColor;
            float _FogStart;
            float _FogEnd;
            float _InterpolationFactor;
            float _Smoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Calculate view direction and angle
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float viewAngle = dot(viewDir, float3(0, 1, 0));
                
                // Interpolate color based on view angle
                float angleFactor = smoothstep(_InterpolationFactor - _Smoothness, _InterpolationFactor + _Smoothness, viewAngle);
                float4 angleColor = lerp(_DepthColor, _SurfaceColor, angleFactor);

                // Calculate distance-based fog
                float distance = length(i.worldPos - _WorldSpaceCameraPos);
                float fogFactor = saturate((distance - _FogStart) / (_FogEnd - _FogStart));

                // Combine angle-based color with distance-based fog
                float4 color = lerp(angleColor, float4(0,0,0,1), fogFactor);
                
                return color;
            }
            ENDCG
        }
    }
}
