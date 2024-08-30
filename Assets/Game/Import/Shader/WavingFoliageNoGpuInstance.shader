Shader "Custom/WavingFoliageNoGpuInstance" {
	Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

		_WaveSpeed("Wave Speed", float) = 1.0
		_WaveAmp("Wave Amp", float) = 1.0
		_WindSkewness("Wind Skewness", float) = 1.0
		_CenterWeight("Center Weight", float) = 0.0
        _WindTex ("Wind Tex", 2D) = "white" {}
        _WindTexStrength ("Wind Tex Strength", float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            
            #include "UnityCG.cginc"

            fixed4 _Color;
            float4 _MainTex_ST;
            sampler2D _MainTex;
            
			float _WaveSpeed;
			float _WaveAmp;
			float _WindSkewness;
			float _CenterWeight;

			sampler2D _WindTex;
			float4 _WindTex_ST;
			float _WindTexStrength;
			uniform float _WindStrength = 2.0;
			uniform Vector _WindTexScrollSpeedXY = (0,0,0,0);

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

				float4 worldPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
				// float2 windTexUV = worldPos.xy * _WindTex_ST.xy + _WindTex_ST.zw;// TRANSFORM_TEX(worldPos.xy, _WindTex);
				float2 windTexUV =  TRANSFORM_TEX(worldPos.xy, _WindTex);
				float offset = tex2Dlod(_WindTex, float4(windTexUV + _Time.y * _WindTexScrollSpeedXY, 0.0, 0.0)).r;

                OUT.vertex = IN.vertex;
				OUT.vertex.x = OUT.vertex.x + (OUT.vertex.y - _CenterWeight) * (sin(_WaveSpeed * (offset * _WindTexStrength + _Time.y) * _WindStrength) * _WaveAmp + _WindSkewness * _WindStrength);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);

                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;

                return OUT;
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
				return tex2D (_MainTex, IN.texcoord) * IN.color;
            }
        ENDCG
        }
    }
}