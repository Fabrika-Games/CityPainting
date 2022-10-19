#ifndef AWD_CORE_INCLUDED
#define AWD_CORE_INCLUDED

#include "UnityCG.cginc"

struct Input
{
	float2 uv_MainTex;
	float2 uv_NoiseTex;
	float2 uv_BumpMap;
	float2 uv_SecondaryTex;
	float3 worldPos;
	float3 stripeUvw;
};

float CubicFade (float3 wldpos, float3 center, float visDist, float fadeWidth)
{
	float distX = distance(wldpos.x, center.x);
	float distY = distance(wldpos.y, center.y);
	float distZ = distance(wldpos.z, center.z);
	float fadeX = smoothstep(visDist, visDist + fadeWidth, distX);
	float fadeY = smoothstep(visDist, visDist + fadeWidth, distY);
	float fadeZ = smoothstep(visDist, visDist + fadeWidth, distZ);
	float f = max(fadeX, fadeZ);
	f = max(f, fadeY);
	return f;
}
float SphericalFade (float3 wldpos, float3 center, float visDist, float fadeWidth)
{
	float dist = distance(wldpos, center);
	return smoothstep(visDist, visDist + fadeWidth, dist);
}
float CylinderFade (float3 wldpos, float3 center, float visDist, float fadeWidth)
{
	float dist = distance(wldpos.xz, center.xz);
	return smoothstep(visDist, visDist + fadeWidth, dist);
}

sampler2D _MainTex, _BumpMap, _SecondaryTex;
float3 _Pos1, _Pos2, _Pos3, _Pos4, _FadeColor;
float  _VisDist, _FadeWidth, _FadeIntensity, _Glossiness, _Metallic, _BumpScale;

float Fade (float3 wldpos)
{
	float f = 0.0, f1 = 0.0, f2 = 0.0, f3 = 0.0, f4 = 0.0;
#ifdef AWD_SPHERICAL
	f1 = SphericalFade(wldpos, _Pos1, _VisDist, _FadeWidth);
	f2 = SphericalFade(wldpos, _Pos2, _VisDist, _FadeWidth);
	f3 = SphericalFade(wldpos, _Pos3, _VisDist, _FadeWidth);
	f4 = SphericalFade(wldpos, _Pos4, _VisDist, _FadeWidth);
#endif
#ifdef AWD_CUBIC
	f1 = CubicFade(wldpos, _Pos1, _VisDist, _FadeWidth);
	f2 = CubicFade(wldpos, _Pos2, _VisDist, _FadeWidth);
	f3 = CubicFade(wldpos, _Pos3, _VisDist, _FadeWidth);
	f4 = CubicFade(wldpos, _Pos4, _VisDist, _FadeWidth);
#endif
#ifdef AWD_ROUND_XZ
	f1 = CylinderFade(wldpos, _Pos1, _VisDist, _FadeWidth);
	f2 = CylinderFade(wldpos, _Pos2, _VisDist, _FadeWidth);
	f3 = CylinderFade(wldpos, _Pos3, _VisDist, _FadeWidth);
	f4 = CylinderFade(wldpos, _Pos4, _VisDist, _FadeWidth);
#endif

#ifdef AWD_POS1
	f = f1;
#endif
#ifdef AWD_POS2
	f = min(f1, f2);
#endif
#ifdef AWD_POS3
	f = min(f1, f2);
	f = min(f, f3);
#endif
#ifdef AWD_POS4
	f = min(f1, f2);
	f = min(f, f3);
	f = min(f, f4);
#endif

	return f;
}

//float3 _ConePos, _ConeNormal;
//float _ConeHeight, _ConeRadius;
//float FadeCone (float3 wldpos, float noise)
//{
//	float3 p1 = _ConePos;
//	float3 p2 = p1 + normalize(_ConeNormal) * _ConeHeight;
//	float t = max(0, min(1, dot(wldpos - p1, p2 - p1) / (_ConeHeight * _ConeHeight)));
//	float3 proj = p1 + t * (p2 - p1);
//	float d = distance(wldpos, proj);

//	float radius = lerp(0, _ConeRadius, t);
//	radius -= noise;
////float cutout = max(0, d - radius);
//	float cutout = radius - min(d, radius);
//	return (cutout > 0 ? cutout : -1);
//}

float PseudoRandom (float2 v)
{
	return frac(sin(dot(v, float2(12.9898, 78.233))) * 43758.5453123);
}
#endif