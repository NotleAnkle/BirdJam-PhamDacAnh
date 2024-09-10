Shader "Toon/ToonRampDitheringByCamAngleUnlit" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" { }
        [Gradient(64)] _Gradient("Gradient", 2D) = "white" {}
		_LightRatio("Light Ratio", float) = 1
        // _Dithering ("Dithering", Range(0, 1)) = 0.5
		_DitheringCamDirRangeXYMappingZW ("Dithering Cam Dir Range XY MappingZW", Vector) = (0, 1, 0, 1)
	}

	SubShader {
		Pass 
		{
			Name "TOON"
			Tags {
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
				"RenderType" = "Opaque" 
				"Queue" = "Geometry"
			}

			ZWrite On
			ColorMask RGB

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"
			
			UNITY_DECLARE_TEX2D_FLOAT(_Gradient);

			struct appdata 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float3 worldNormal : NORMAL;
                float4 spos : TEXCOORD1;
                SHADOW_COORDS(2) // put shadows data into TEXCOORD2
			};
	
			sampler2D _MainTex;
			half _LightRatio;
			uniform fixed4 _GAmbientColor;
            // float _Dithering;
            float4 _DitheringCamDirRangeXYMappingZW;
			
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

			v2f vert(appdata v) 
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(v.vertex);
				OUT.worldNormal = UnityObjectToWorldNormal(v.normal);
				OUT.texcoord = v.texcoord;
                OUT.spos = ComputeScreenPos(OUT.vertex);
                TRANSFER_SHADOW(OUT)
				return OUT;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float dithering = smoothstep(_DitheringCamDirRangeXYMappingZW.x, _DitheringCamDirRangeXYMappingZW.y, UNITY_MATRIX_IT_MV[2].y);
				dithering = lerp(_DitheringCamDirRangeXYMappingZW.z, _DitheringCamDirRangeXYMappingZW.w, dithering);

                ditherClip(i.spos.xy / i.spos.w, dithering);

				float3 normal = normalize(i.worldNormal);
				fixed4 color = tex2D(_MainTex, i.texcoord);

				// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
                fixed shadow = SHADOW_ATTENUATION(i);

				float NdotL = dot(_WorldSpaceLightPos0, normal);
				half d = NdotL * 0.5 + 0.5;
				fixed3 lightIntensity = UNITY_SAMPLE_TEX2D(_Gradient, float2(d, 0.5f)).rgb * _LightRatio;

				color = float4(color.rgb * (_GAmbientColor.rgb + lightIntensity * _LightColor0.rgb * shadow), color.a);
			
				return color;
			}

			ENDCG
		}

		// UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		Pass 
		{
			Name "SHADOW CAST DITHERING"
			Tags {
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
            float _DitheringCamDirXRangeMin;
            float _DitheringCamDirXRangeMax;
			
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

			struct v2f { 
                V2F_SHADOW_CASTER;
                float4 spos     : TEXCOORD1;
            };

			v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				o.spos = ComputeScreenPos(o.pos);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
				float dithering = smoothstep(_DitheringCamDirXRangeMin, _DitheringCamDirXRangeMax, UNITY_MATRIX_IT_MV[2].y);
                ditherClip(i.spos.xy / i.spos.w, dithering);
                SHADOW_CASTER_FRAGMENT(i)
            }

			ENDCG
		}
	}

	Fallback "Diffuse"
}