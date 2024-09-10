Shader "Toon/LitGradientEmissive" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EmissiveTex ("Emmisive", 2D) = "white" {}
        [Gradient(64)] _Gradient("Gradient", 2D) = "white" {}
		_LightRatio ("Light Ratio", float) = 1
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma vertex vert
		#pragma surface surf ToonRamp fullforwardshadows addshadow
		
		UNITY_DECLARE_TEX2D_FLOAT(_Gradient);

		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
		#pragma lighting ToonRamp exclude_path:prepass
		inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
		{
			#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
			#endif
	
			float d = dot (s.Normal, lightDir)*0.5 + 0.5;
			half3 ramp = UNITY_SAMPLE_TEX2D(_Gradient, float2(d, 0.5f)).rgb;
	
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			c.a = 0;
			return c;
		}

		sampler2D _MainTex;
		sampler2D _EmissiveTex;
		float4 _Color;
		half _LightRatio;

		const float PI = 3.1415926;
		const float PI_DIV_2 = 1.5707963;

		float4 _WavePosXY_AmpZ_FreqW;
		// uniform float4 _WavePosXY_AmpZ_FreqW = (0,0,1,1);
		float4 _WaveTimeX_SpeedY_DecayZ_TimeOffsetW;
		// uniform float4 _WaveTimeX_SpeedY_DecayZ = (5,1,0.1,0);

		struct Input {
			float2 uv_MainTex : TEXCOORD0;
		};

		void vert (inout appdata_full v)
        {
            float4 world = mul(unity_ObjectToWorld, v.vertex);
			float dist = distance(world.xz, _WavePosXY_AmpZ_FreqW.xy);

			float amp = _WavePosXY_AmpZ_FreqW.z;
			float freq = _WavePosXY_AmpZ_FreqW.w;

			float moveTime = dist / _WaveTimeX_SpeedY_DecayZ_TimeOffsetW.y;
			float time = _Time.y - _WaveTimeX_SpeedY_DecayZ_TimeOffsetW.x - moveTime;
			time = (time > 0.0) ? time : 0.0;

			world.y += sin(time * freq + _WaveTimeX_SpeedY_DecayZ_TimeOffsetW.w) * amp * exp(-time * _WaveTimeX_SpeedY_DecayZ_TimeOffsetW.z);

            float4 vertex = mul(unity_WorldToObject, world);
            v.vertex = vertex;
        }

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * _LightRatio;
			half4 e = tex2D(_EmissiveTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Emission = e.rgb * e.a;
			o.Alpha = c.a;
		}
		ENDCG

	} 

	Fallback "Diffuse"
}