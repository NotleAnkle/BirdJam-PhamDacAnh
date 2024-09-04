Shader "Toon/LitGradientEmissiveBlendReal" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EmissiveTex ("Emmisive", 2D) = "white" {}
        [Gradient(64)] _Gradient("Gradient", 2D) = "white" {}
		_RealLightBlendRatio ("Real Light Blend Ratio", float) = 0.5
		_EmissiveBlendRatio ("Emissive Blend Ratio", float) = 0.1
		_LightRatio ("Light Ratio", float) = 1
		_EmissiveTintColor ("Emissive Tint Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows addshadow
		
		UNITY_DECLARE_TEX2D_FLOAT(_Gradient);

		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
		#pragma lighting ToonRamp exclude_path:prepass

		half _RealLightBlendRatio;
		half _EmissiveBlendRatio;

		inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
		{
			#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
			#endif
	
			float d = dot (s.Normal, lightDir)*0.5 + 0.5;
			half3 ramp = UNITY_SAMPLE_TEX2D(_Gradient, float2(d, 0.5f)).rgb;
			ramp = lerp(ramp, d, _RealLightBlendRatio);
	
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			c.a = 1;
			return c;
		}

		sampler2D _MainTex;
		sampler2D _EmissiveTex;
		float4  _EmissiveTex_ST;
		fixed4 _Color;
		fixed4 _EmissiveTintColor;
		half _LightRatio;

		struct Input {
			float2 uv_MainTex : TEXCOORD0;
            float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * _LightRatio;
			float2 e_uv = TRANSFORM_TEX(IN.uv_MainTex, _EmissiveTex);
			half4 e = tex2D(_EmissiveTex, e_uv);
			o.Albedo = c.rgb;
			o.Emission = lerp(e.rgb * e.a * _EmissiveTintColor.rgb, c.rgb, _EmissiveBlendRatio);
			o.Alpha = c.a;
		}
		ENDCG

	} 

	Fallback "Diffuse"
}