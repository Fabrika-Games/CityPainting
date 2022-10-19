Shader "Amazing World Fading/Pattern Sphere" {
	Properties {
		[Header(Standard)]
		_Albedo                        ("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_Normal ("Normal", 2D) = "bump" {}
		_Metallic                      ("Metallic", Range(0, 1)) = 0
		_Smoothness                    ("Smoothness", Range(0, 1)) = 0
		[Header(Pattern)]
		_Pos              ("Position", Vector) = (0, 0, 0, 0)
		_Radius           ("Radius", Float) = 2
		[Toggle]_Invert   ("Invert", Float) = 0
		_NoiseSpeed       ("Noise Speed", Vector) = (0, 0, 0, 0)
		_BorderRadius     ("Border Radius", Range(0, 2)) = 1
		_BorderNoiseScale ("Border Noise Scale", Range(0, 4)) = 0.6
		[HDR]_BorderColor ("Border Color", Color) = (0.86, 0.2, 0.2, 1)
		_Cutoff           ("Cutoff", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "RenderType" = "TransparentCutout" "Queue" = "Geometry" }
		Cull Back

		CGPROGRAM
		#include "Noise.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_Albedo;
			float3 worldPos;
		};
		sampler2D _Albedo, _Normal;
		float4 _BorderColor;
		float3 _Pos, _NoiseSpeed;
		float _Radius, _Invert, _BorderRadius, _BorderNoiseScale, _Cutoff, _Smoothness, _Metallic;

		void surf (Input input, inout SurfaceOutputStandard o)
		{
			float2 uv = input.uv_Albedo;
			half4 albedo = tex2D(_Albedo, uv);
			o.Normal = UnpackNormal(tex2D(_Normal, uv));
			o.Albedo = albedo.rgb;

			float3 wldpos = input.worldPos;
			float d = distance(_Pos, wldpos);
			float nis = snoise(_BorderNoiseScale * (wldpos + _NoiseSpeed * _Time.y)) + _Radius;
			float var1 = step(1.0 - saturate(d / nis) , 0.5);
			float var2 = saturate(d / (_BorderRadius + nis));
			float var3 = var1 - step(1.0 - var2, 0.5);
			o.Emission = (_BorderColor * var3).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = albedo.a;

			float lp = lerp(var1, step(var2 , 0.5), _Invert);
			clip(lp - _Cutoff);
		}
		ENDCG
	}
	Fallback "Diffuse"
}