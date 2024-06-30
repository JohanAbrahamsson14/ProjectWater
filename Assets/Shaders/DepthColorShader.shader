Shader "Custom/DepthColorShader"
{
    Properties
    {
        _UpColor ("Up Color", Color) = (0,1,0,1) // Green for up
        _DownColor ("Down Color", Color) = (1,0,0,1) // Red for down
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            float4 _UpColor;
            float4 _DownColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.viewDir = normalize(UnityWorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float angle = dot(i.viewDir, float3(0,1,0));
                float t = (angle + 1) / 2; // Adjusting range to [0, 1]
                fixed4 color = lerp(_DownColor, _UpColor, t);
                return color;
            }
            ENDCG
        }
    }
}
