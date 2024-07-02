Shader "Custom/FogShader"
{
    Properties
    {
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

            sampler2D _MainTex;
            float4 _DepthColor;
            float4 _SurfaceColor;
            float _FogStart;
            float _FogEnd;
            float _InterpolationFactor;
            float _Smoothness;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float distance = length(i.worldPos.xyz - _WorldSpaceCameraPos);
                float fogFactor = saturate((distance - _FogStart) / (_FogEnd - _FogStart));

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
                float viewAngle = dot(viewDir, float3(0, 1, 0));
                float t = smoothstep(_InterpolationFactor - _Smoothness, _InterpolationFactor + _Smoothness, viewAngle);

                float4 color = lerp(_DepthColor, _SurfaceColor, t);
                color = lerp(color, float4(0, 0, 0, 1), fogFactor);

                return color;
            }
            ENDCG
        }
    }
}
