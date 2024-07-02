Shader "Custom/HighPrecisionUnderwaterShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DepthColor ("Depth Color)", Color) = (0, 0.2, 0.5, 1)
        _SurfaceColor ("Surface Color", Color) = (0.2, 0.8, 1, 1)
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
            #pragma target 3.0
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
            half4 _DepthColor;
            half4 _SurfaceColor;
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
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float viewAngle = dot(viewDir, float3(0, 1, 0));

                // Apply the smoothstep function for smoother interpolation
                float t = smoothstep(_InterpolationFactor - _Smoothness, _InterpolationFactor + _Smoothness, viewAngle);

                // Apply color interpolation
                half4 color = lerp(_DepthColor, _SurfaceColor, t);

                return color;
            }
            ENDCG
        }
    }
}
