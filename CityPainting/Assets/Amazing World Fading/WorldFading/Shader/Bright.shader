Shader "Amazing World Fading/Bright" {
	Properties {
		[Header(Standard)][Space(5)]
		_MainTex       ("Albedo", 2D) = "white" {}
		_BumpMap       ("Normal", 2D) = "bump" {}
		_Glossiness    ("Smoothness", Range(0, 1)) = 0.5
		_Metallic      ("Metallic", Range(0, 1)) = 0
		[Header(Bright)][Space(5)]
		_Pos           ("Position", Vector) = (0, 0, 0, 0)
		_VisDist       ("Visibility Distance", Range(0.01, 16)) = 8
		_FadeWidth     ("Fade Width", Range(0, 8)) = 2
		_FadeColor     ("Fade Color", Color) = (1, 1, 1, 1)
		_FadeIntensity ("Fade Intensity", Range(0, 1)) = 0.7
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0
		#pragma multi_compile AWD_SPHERICAL AWD_CUBIC AWD_ROUND_XZ
		#pragma multi_compile _ AWD_INVERSE
		#pragma multi_compile AWD_POS1 AWD_POS2 AWD_POS3 AWD_POS4
		#include "Core.cginc"
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			float fade = Fade(IN.worldPos);
#ifdef AWD_INVERSE
			fade = 1.0 - fade;
#endif
			o.Albedo = lerp(c, c * _FadeIntensity, fade);
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}