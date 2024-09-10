// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Mobile/SpriteWithRoughnessMap"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_ReflectionTex ("Reflection Texture", 2D) = "white" {}

		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimSmooth("Rim Smooth", Range(0, 0.5)) = 0.01

        [Gradient(64)] _Gradient("Gradient", 2D) = "white" {}
		_RealLightBlendRatio ("Real Light Blend Ratio", float) = 0.5

		_Ambient ("Ambient", float) = 0.1
		_EmissiveTex ("Emmisive", 2D) = "white" {}
		_EmissiveTintColor ("Emissive Tint Color", Color) = (1,1,1,1)
	}

	SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"
            
		    UNITY_DECLARE_TEX2D_FLOAT(_Gradient);

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
			};

            struct v2f {
                float4 pos : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
                half3 worldNormal : TEXCOORD1;
                half3 worldViewDir : TEXCOORD2;
                SHADOW_COORDS(3) // put shadows data into TEXCOORD3
            };

			sampler2D _MainTex;
			sampler2D _ReflectionTex;
			fixed4 _Color;
            
		    fixed4 _RimColor;
		    float _RimAmount;
		    float _RimSmooth;
		    float _Ambient;

		    float _RealLightBlendRatio;
            
		    sampler2D _EmissiveTex;
		    float4  _EmissiveTex_ST;
		    fixed4 _EmissiveTintColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
                // compute world space position of the vertex
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // compute world space view direction
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                // world space normal
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // world space reflection vector
                o.worldNormal = worldNormal; //reflect(-worldViewDir, worldNormal);
                o.worldViewDir = worldViewDir;
                TRANSFER_SHADOW(o)
                return o;
            }
        
            fixed4 frag (v2f i) : SV_Target
            {
			    float d = dot (i.worldNormal, _WorldSpaceLightPos0.xyz)*0.5 + 0.5 + _Ambient;
			    half3 ramp = UNITY_SAMPLE_TEX2D(_Gradient, float2(d, 0.5f)).rgb;
			    ramp = lerp(ramp, d, _RealLightBlendRatio);

                // compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
                fixed shadow = SHADOW_ATTENUATION(i);

                // sample the default reflection cubemap, using the reflection vector
                half3 worldRefl = reflect(-i.worldViewDir, i.worldNormal);
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
                // decode cubemap data into actual color
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);
                // output it!
                fixed4 c = tex2D(_MainTex, i.texcoord) * _Color;
                fixed reflectionFactor = tex2D(_ReflectionTex, i.texcoord).r;

                c.rgb = lerp(c.rgb, skyColor, reflectionFactor) * ramp * shadow;

                half rim = 1.0 - saturate(dot(i.worldViewDir, i.worldNormal));
			    rim = smoothstep(_RimAmount - _RimSmooth, _RimAmount + _RimSmooth, rim);

                float2 e_uv = TRANSFORM_TEX(i.texcoord, _EmissiveTex);
			    half4 emissive = tex2D(_EmissiveTex, e_uv) * _EmissiveTintColor;

                c.rgb = lerp(c.rgb, _RimColor.rgb, rim * _RimColor.a) + emissive.rgb * emissive.a;

                return c;
            }
            ENDCG
        }

		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}