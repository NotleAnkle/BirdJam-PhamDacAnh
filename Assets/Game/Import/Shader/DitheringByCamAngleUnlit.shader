Shader "Toon/DitheringByCamAngleUnlit"
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _Dithering ("Dithering", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;
            float4 _MainTex_ST;         // For the Main Tex UV transform
            sampler2D _MainTex;         // Texture used for the line
            float _Dithering;
            
            struct v2f
            {
                float4 pos      : POSITION;
                float2 uv       : TEXCOORD0;
                float4 spos     : TEXCOORD1;
            };

            static const float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };

            float isDithered(float2 pos, float alpha) 
            {
                pos *= _ScreenParams.xy;
                int index = (int(pos.x) % 4) * 4 + int(pos.y) % 4;
                return alpha - DITHER_THRESHOLDS[index];
            }

            void ditherClip(float2 pos, float alpha) 
            {
                clip(isDithered(pos, alpha));
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.spos = ComputeScreenPos(o.pos);
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                float4 col = _Color * tex2D(_MainTex, i.uv);
                ditherClip(i.spos.xy / i.spos.w, _Dithering);
                return col;
            }

            ENDCG
        }
    }
}