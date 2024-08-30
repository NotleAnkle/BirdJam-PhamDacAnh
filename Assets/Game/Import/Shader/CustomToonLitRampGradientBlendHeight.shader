Shader "Toon/LitRampGradientBlendHeight" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" { }
		_EmissiveTex("Emmisive", 2D) = "white" { }
        [Gradient(64)] _Gradient("Gradient", 2D) = "white" {}
		_LightRatio("Light Ratio", float) = 1
		_GrayscaleSmoothXHeightYBlendZ("GrayScale Smooth X Height Y Blend Z", Vector) = (0.1, 0, 1.0, 0)
	}

	SubShader {
		Pass 
		{
			Name "TOON"
			Tags {
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			ZWrite On
			ColorMask RGB

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
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
				float3 viewDir : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};
	
			sampler2D _MainTex;
			sampler2D _EmissiveTex;
			sampler2D _Ramp;
			half _LightRatio;
			uniform fixed4 _GAmbientColor;
			float4 _GrayscaleSmoothXHeightYBlendZ;

			v2f vert(appdata IN) 
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
				OUT.texcoord = IN.texcoord;
				OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
				OUT.viewDir = WorldSpaceViewDir(IN.vertex);
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float3 normal = normalize(IN.worldNormal);
				fixed4 color = tex2D(_MainTex, IN.texcoord);
				fixed4 emitColor = tex2D(_EmissiveTex, IN.texcoord);

				float NdotL = dot(_WorldSpaceLightPos0, normal);
				half d = NdotL * 0.5 + 0.5;
				fixed3 lightIntensity = UNITY_SAMPLE_TEX2D(_Gradient, float2(d, 0.5f)).rgb * _LightRatio;

				float blendFactor = saturate((IN.worldPos.y - _GrayscaleSmoothXHeightYBlendZ.y) / _GrayscaleSmoothXHeightYBlendZ.x) * _GrayscaleSmoothXHeightYBlendZ.z;

				color = float4(color.rgb * (_GAmbientColor.rgb + lightIntensity * _LightColor0.rgb * (1.0 - blendFactor)) + emitColor.rgb * emitColor.a * (1.0 - blendFactor), color.a);
			
				fixed grayScale = dot(color.rgb, float3(0.299, 0.587, 0.114));

				return lerp(color, grayScale, blendFactor);
			}

			ENDCG
		}
	}

	Fallback "Diffuse"
}