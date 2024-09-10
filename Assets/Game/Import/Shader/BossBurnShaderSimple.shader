// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/BossBurnShaderSimple"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		_NoiseTex("Noise", 2D) = "white" {}
		_NoiseDistortScroll("Distort Scroll XY, Add Strength Z, Mul Strength W", Vector) = (1.0, 1.0, 1.0, 1.0)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_EdgeColour("Edge colour", Color) = (1.0, 1.0, 1.0, 1.0)
		_Level("Dissolution level", Range(-0.1, 1.1)) = 0.1
		_Edges("Edge width", Range(0.0, 1.0)) = 0.1
		_TintBurnedTex("Tint Destroyed Origin Tex", Color) = (0.0, 0.0, 0.0, 1.0)
		_GlowColour("Glow colour", Color) = (1.0, 1.0, 1.0, 1.0)
		_GlowRange("Glow Range", Float) = 0.1

	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//// make fog work
			//#pragma multi_compile DUMMY PIXELSNAP_ON
			#pragma multi_compile DUMMY USEBURNTEX_ON

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 noise_uv : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 vertexColor : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;

			float4 _NoiseDistortScroll;
			float4 _EdgeColour;
			float4 _TintBurnedTex;
			float _Level;
			float _Edges;

			float4 _GlowColour;
			float _GlowRange;

			inline half3 GammaToTargetSpace(half3 gammaColor) {
#if UNITY_COLORSPACE_GAMMA
				return gammaColor;
#else
				return GammaToLinearSpace(gammaColor);
#endif
			}

			inline half3 TargetToGammaSpace(half3 targetColor) {
#if UNITY_COLORSPACE_GAMMA
				return targetColor;
#else
				return LinearToGammaSpace(targetColor);
#endif
			}

			inline half4 PMAGammaToTargetSpace(half4 gammaPMAColor) {
#if UNITY_COLORSPACE_GAMMA
				return gammaPMAColor;
#else
				return gammaPMAColor.a == 0 ?
					half4(GammaToLinearSpace(gammaPMAColor.rgb), gammaPMAColor.a) :
					half4(GammaToLinearSpace(gammaPMAColor.rgb / gammaPMAColor.a) * gammaPMAColor.a, gammaPMAColor.a);
#endif
			}

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.noise_uv = TRANSFORM_TEX(v.uv, _NoiseTex);
				o.vertexColor = PMAGammaToTargetSpace(v.vertexColor);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				float cutout = tex2D(_NoiseTex, i.noise_uv).r;
				float distort = tex2D(_NoiseTex, i.noise_uv * float2(43.0, 41.0) + _NoiseDistortScroll.xy * _Time).r - 0.5;
				cutout = cutout + distort * _NoiseDistortScroll.z;

				i.vertexColor.a = 1.0;
				fixed4 col = tex2D(_MainTex, i.uv) * i.vertexColor;
				fixed4 burnedCol = col * _TintBurnedTex;

				float glow = clamp(1.0 - abs(cutout - _Level) / _GlowRange + distort * _NoiseDistortScroll.w, 0.0, 1.0);
				glow = glow * glow * glow * col.a;

				/*if (cutout < _Level)
					discard;
				if (cutout < col.a && cutout < _Level + _Edges)
					col = lerp(_EdgeColour1, _EdgeColour2, (cutout - _Level) / _Edges);*/

				if (cutout < _Level) {
					col = burnedCol;
				}
				else 
				{
					if (cutout < col.a && cutout < _Level + _Edges) {
						col = lerp(float4(0.0,0.0,0.0,0.0), _EdgeColour, (cutout - _Level) / _Edges);
						//col = lerp(burnedCol, col, col.a);
					}
				}

				return col + glow * _GlowColour;
			}
			ENDCG
		}
	}
}