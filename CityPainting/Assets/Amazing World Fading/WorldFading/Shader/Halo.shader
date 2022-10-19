Shader "Amazing World Fading/Halo" {
	Properties {
		[Header(Standard)][Space(5)]
		_MainTex    ("Albedo", 2D) = "white" {}
		_BumpScale  ("Normal Scale", Float) = 1.0
		_BumpMap    ("Normal", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic   ("Metallic", Range(0, 1)) = 0
		[Header(Halo)][Space(5)]
		_Pos        ("Position", Vector) = (0, 0, 0, 0)
		_VisDist    ("Visibility Distance", Range(0.01, 16)) = 8
		_FadeWidth  ("Fade Width", Range(0, 8)) = 2
		_FadeColor  ("Fade Color", Color) = (1, 1, 1, 1)
		_NoiseTex   ("Noise", 2D) = "white" {}
		_NoiseSize  ("Noise size", Float) = 1
		_FadePartAlpha   ("Fading Part Alpht", Range(0, 1)) = 0
		_FadeToColor     ("Fading To Color", Color) = (1, 1, 1, 1)
		_FadeToColorLerp ("Fading To Color Lerp", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" }
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Standard keepalpha addshadow
		#pragma target 3.0
		#pragma multi_compile AWD_SPHERICAL AWD_CUBIC AWD_ROUND_XZ
		#pragma multi_compile _ AWD_INVERSE
		#pragma multi_compile _ AWD_NOISEHALO
		#pragma multi_compile AWD_POS1 AWD_POS2 AWD_POS3 AWD_POS4
		#include "Core.cginc"

		sampler2D _NoiseTex;
		half _NoiseSize, _HaloBloom, _FadePartAlpha, _FadeToColorLerp;
		half4 _FadeToColor;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float fade = Fade(IN.worldPos);
#ifdef AWD_INVERSE
			fade = 1.0 - fade;
#endif
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			c *= 2.0;
			if (fade < 0.01)
			{
				o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpScale);
				o.Albedo = c.rgb;
				o.Alpha = 1.0;
			}
			else if (fade > 0.01 && fade < 0.99)
			{
#ifdef AWD_NOISEHALO
				float ns = tex2D(_NoiseTex, IN.uv_NoiseTex * _NoiseSize).r;
				o.Alpha = (1.0 - fade) * ns;
#else
				o.Alpha = 1.0 - fade;
#endif
				o.Albedo = 0.0;
				o.Emission = _FadeColor * _HaloBloom;
			}
			else
			{
				o.Albedo = lerp(c.rgb, _FadeToColor, _FadeToColorLerp);
				o.Alpha = _FadePartAlpha;
//				o.Emission = lerp(0.0, _FadeToColor, _FadeToColorLerp);
			}
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}