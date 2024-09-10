Shader "Custom/TwoSidedTexture"
{
    Properties
    {
        _MainTex ("Texture Front", 2D) = "white" {}
        _MainColor ("Color Front Blend", Color) = (1,1,1,0)
        _SecondTex ("Texture Back", 2D) = "white" {}
        _SecondColor ("Color Back Blend", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            sampler2D _SecondTex;
            fixed4 _MainColor;
            fixed4 _SecondColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i, bool isFacing: SV_IsFrontFace) : SV_Target
            {
                fixed4 tex = !isFacing ? tex2D(_MainTex, i.uv) : tex2D(_SecondTex, i.uv);
                fixed4 blendColor = (i.normal.z > 0) ? _MainColor : _SecondColor;
                tex.rgb = lerp(tex.rgb, blendColor.rgb, blendColor.a);
                return tex;
            }
            ENDCG
        }
    }
}
