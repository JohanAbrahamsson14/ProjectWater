Shader "Custom/SimpleUnderwaterShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DepthColor ("Depth Color", Color) = (0, 0.2, 0.5, 1)
        _SurfaceColor ("Surface Color", Color) = (0.2, 0.8, 1, 1)
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
                float viewAngle = dot(viewDir, float3(0, 1, 0));
                
                float4 color = lerp(_DepthColor, _SurfaceColor, viewAngle);
                
                return color;
            }
            ENDCG
        }
    }
}
