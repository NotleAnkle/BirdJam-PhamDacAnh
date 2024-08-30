Shader "Custom/WavingFoliageWithNightFlashLight" {
	Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        
        [PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
        
        _ShadowBlend("Shadow Blend", Color) = (0,0,0,1)
        _FlashLightTex ("Flash Light Texture", 2D) = "white" {}

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
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            
			#ifndef UNITY_SPRITES_INCLUDED
            #define UNITY_SPRITES_INCLUDED

            #include "UnityCG.cginc"

            #ifdef UNITY_INSTANCING_ENABLED

                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                    // SpriteRenderer.Color while Non-Batched/Instanced.
                    UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                    // this could be smaller but that's how bit each entry is regardless of type
                    UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
                    UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
                    UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

                #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
                #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)
                #define _Color          UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, _Color)
                #define _MainTex_ST     UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, _MainTex_ST)

            #endif // instancing

            CBUFFER_START(UnityPerDrawSprite)
            #ifndef UNITY_INSTANCING_ENABLED
                fixed4 _RendererColor;
                fixed2 _Flip;
                fixed4 _Color;
                float4 _MainTex_ST;
            #endif
                float _EnableExternalAlpha;
            CBUFFER_END
            
			fixed4 _ShadowBlend;
            sampler2D _FlashLightTex;
            uniform float4 _FlashLightSourcePos;
            uniform float4 _FlashLightSourceScaleXYRotateCacheSinCosZW;
            static const float2 FLASH_LIGHT_PIVOT = float2(0.5, 0.0);

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 flashLightCoord : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                // UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
				float4 worldPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
				// float2 windTexUV = worldPos.xy * _WindTex_ST.xy + _WindTex_ST.zw;// TRANSFORM_TEX(worldPos.xy, _WindTex);
				float2 windTexUV =  TRANSFORM_TEX(worldPos.xy, _WindTex);
				float offset = tex2Dlod(_WindTex, float4(windTexUV + _Time.y * _WindTexScrollSpeedXY, 0.0, 0.0)).r;

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex.x = OUT.vertex.x + (OUT.vertex.y - _CenterWeight) * (sin(_WaveSpeed * (offset * _WindTexStrength + _Time.y) * _WindStrength) * _WaveAmp + _WindSkewness * _WindStrength);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);

                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                // OUT.texcoord = IN.texcoord;
                // OUT.color = IN.color * _RendererColor;
                OUT.color = IN.color * _Color * _RendererColor;

                // Flash Light
                float4 vWorldPos = mul(unity_ObjectToWorld, IN.vertex);
                float2 deltaPos = (vWorldPos.xy - _FlashLightSourcePos.xy);
                float2 rotatedDeltaPos = float2(
                    deltaPos.x * _FlashLightSourceScaleXYRotateCacheSinCosZW.w - deltaPos.y * _FlashLightSourceScaleXYRotateCacheSinCosZW.z, // x*cos-y*sin
                    deltaPos.x * _FlashLightSourceScaleXYRotateCacheSinCosZW.z + deltaPos.y * _FlashLightSourceScaleXYRotateCacheSinCosZW.w // x*sin+y*cos
                    );
                float2 transformedDeltaPos = rotatedDeltaPos / _FlashLightSourceScaleXYRotateCacheSinCosZW.xy;
                OUT.flashLightCoord = transformedDeltaPos + FLASH_LIGHT_PIVOT;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);

            #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D (_AlphaTex, uv);
                color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
            #endif

                return color;
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                fixed4 flashLightColor = tex2D(_FlashLightTex, IN.flashLightCoord);
                fixed shadowBlendFactor = _ShadowBlend.a * (1.0 - flashLightColor.a);
                c.rgb = lerp(c.rgb, _ShadowBlend.rgb, shadowBlendFactor);
				return c * c.a;
            }

            #endif // UNITY_SPRITES_INCLUDED
        ENDCG
        }
    }
}