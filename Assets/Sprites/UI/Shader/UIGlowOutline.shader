Shader "UI/GlowOutline"
{
    Properties
    {
        _MainTex("Sprite", 2D) = "white" {}
        _GlowColor("Glow Color", Color) = (1,1,1,1)
        _GlowStrength("Glow Strength", Range(0,5)) = 0
        _GlowThickness("Glow Thickness", Range(0,10)) = 2
    }

        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "PreviewType" = "Plane" }
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _GlowColor;
                float _GlowStrength;
                float _GlowThickness;

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;
                    fixed4 col = tex2D(_MainTex, uv);

                    // 기본 알파
                    float alpha = col.a;

                    // Glow (주변 픽셀 샘플링으로 외곽 추출)
                    float glow = 0.0;
                    float2 offsets[4] = {float2(1,0), float2(-1,0), float2(0,1), float2(0,-1)};
                    for (int j = 0; j < 4; j++) {
                        glow += tex2D(_MainTex, uv + offsets[j] * _GlowThickness / 512).a;
                    }

                    glow = saturate(glow - alpha); // 내부 제외, 외곽만
                    fixed4 glowCol = _GlowColor * glow * _GlowStrength;

                    return col + glowCol; // 원본 + 글로우 합성
                }
                ENDCG
            }
        }
}
