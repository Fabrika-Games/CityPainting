Shader "Amazing World Fading/Pattern Vertical" {
	Properties {
		[Header(Standard)]
		_Albedo                        ("Albedo", 2D) = "black" {}
		[NoScaleOffset][Normal]_Normal ("Normal", 2D) = "bump" {}
		_Metallic                      ("Metallic", Range(0, 1)) = 0
		_Smoothness                    ("Smoothness", Range(0, 1)) = 0
		[Header(Pattern)]
		_Fill                    ("Fill", Float) = 1
		_Cutoff                  ("Cutoff", Range(0, 1)) = 0.51
		_BorderWidth             ("Border Width", Range(0, 1)) = 0
		[HDR]_BorderColor        ("Border", Color) = (0, 0, 0, 0)
		[HDR]_FillColor          ("Fill", Color) = (1, 0.5, 0.5, 1)
		_NoiseScale              ("Noise Scale", Range(0, 8)) = 0
		_NoiseSpeed              ("Noise Speed", Vector) = (0, 0, 0, 0)
		[Toggle]_Invert          ("Invert", Float) = 0
		[Toggle]_TintInsideColor ("Tint Inside Color", Float) = 1
		_Wave1amplitude          ("Wave1 Amplitude", Range(0, 1)) = 0
		_Wave1frequency          ("Wave1 Frequency", Range(0, 50)) = 0
		_Wave2amplitude          ("Wave2 Amplitude", Range(0, 1)) = 0
		_Wave2Frequency          ("Wave2 Frequency", Range(0, 50)) = 0
	}
	SubShader {
		Tags { "RenderType" = "TransparentCutout" "Queue" = "Geometry" }
		Cull Off

		CGPROGRAM
		#include "Noise.cginc"
		#pragma target 3.0
		#pragma surface surf Standard addshadow fullforwardshadows

		struct Input
		{
			float2 uv_Albedo;
			float3 worldPos;
			half ASEVFace : VFACE;
		};
		sampler2D _Albedo, _Normal;
		float4 _BorderColor, _FillColor;
		float3 _NoiseSpeed;
		float _Fill, _Cutoff, _BorderWidth, _Invert, _NoiseScale, _Smoothness, _TintInsideColor, _Metallic;
		float _Wave1amplitude, _Wave1frequency, _Wave2amplitude, _Wave2Frequency;

		void surf (Input input, inout SurfaceOutputStandard o)
		{
			float3 objpos = mul(unity_WorldToObject, float4(input.worldPos, 1.0));
			float nis = snoise(_NoiseScale * (objpos + _NoiseSpeed * _Time.y));
			float wave = (_Wave1amplitude * sin(nis + ((nis + objpos.x) * _Wave1frequency))) + objpos.y + (sin(((objpos.z + nis) * _Wave2Frequency) + nis) * _Wave2amplitude);
			float fill = _Fill * 2.0 - 1.0;
			float s1 = step((max(0, _BorderWidth) * 0.1) + wave, fill);
			float ivt = 1.0 - _Invert;
			float s2 = step(wave, fill);
			float mask = (s1 * ivt) + (_Invert * (1.0 - s2));
			float4 border = _BorderColor * (s2 - s1);

			float f = (s2 * ivt) + ((1.0 - s1) * _Invert);
			clip(f - _Cutoff);

			float2 uv = input.uv_Albedo;
			half4 albedo = tex2D(_Albedo, uv);
			o.Albedo = albedo.rgb;
			o.Normal = UnpackNormal(tex2D(_Normal, uv));
			float4 emis = (input.ASEVFace > 0) ? border : (border + (_TintInsideColor * _FillColor * mask));
			o.Emission = emis.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = albedo.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}