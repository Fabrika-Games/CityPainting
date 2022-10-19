Shader "Amazing World Fading/Blend 2 Textures" {
	Properties {
		[Header(Standard)][Space(5)]
		_Glossiness   ("Smoothness", Range(0, 1)) = 0.5
		_Metallic     ("Metallic", Range(0, 1)) = 0
		[Header(BlendTextures)][Space(5)]
		_MainTex      ("Albedo", 2D) = "white" {}
		_SecondaryTex ("Secondary", 2D) = "white" {}
		_Pos1         ("Position1", Vector) = (0, 0, 0, 0)
		_Pos2         ("Position2", Vector) = (0, 0, 0, 0)
		_Pos3         ("Position3", Vector) = (0, 0, 0, 0)
		_Pos4         ("Position4", Vector) = (0, 0, 0, 0)
		_VisDist      ("Visibility Distance", Range(0.01, 16)) = 8
		_FadeWidth    ("Fade Width", Range(0, 8)) = 2
		_FadeColor    ("Fade Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Standard
		#pragma multi_compile AWD_SPHERICAL AWD_CUBIC AWD_ROUND_XZ
		#pragma multi_compile _ AWD_INVERSE
		#pragma multi_compile AWD_POS1 AWD_POS2 AWD_POS3 AWD_POS4
		#include "Core.cginc"
		float _Blend2TexBloom;
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float fade = Fade(IN.worldPos);
			half4 c1 = tex2D(_MainTex, IN.uv_MainTex);
			half4 c2 = tex2D(_SecondaryTex, IN.uv_SecondaryTex);
#ifdef AWD_INVERSE
			fade = 1.0 - fade;
#endif
			if (fade < 0.01)
			{
				o.Albedo = c1.rgb * 2;
				o.Alpha = c1.a;
			}
			else if (fade > 0.01 && fade < 0.99)
			{
				o.Albedo = _FadeColor * _Blend2TexBloom;
				o.Alpha = 1;
			}
			else
			{
				o.Albedo = c2.rgb;
				o.Alpha = c2.a;
			}
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}