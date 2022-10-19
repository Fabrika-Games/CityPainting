Shader "Amazing World Fading/IntersectHighlit" {
	Properties {
		[HDR]_MainColor  ("Main", Color) = (1, 1, 1, 0.25)
		_MainTex         ("Base", 2D) = "white" {}

		[Header(Rim)]
		_RimPower      ("Rim Power", Range(1, 10)) = 2
		_RimIntensity  ("Rim Intensity", Range(1, 6)) = 2

		[Header(Intersection)]
		_IntersectionMax        ("Intersection Max", Float) = 1
		_IntersectionDamper     ("Intersection Damper", Float) = 0.3
		[HDR]_IntersectionColor ("Intersection Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _CameraDepthTexture, _MainTex;
			fixed4 _MainColor, _IntersectionColor, _RimColor;
			half _IntersectionMax, _IntersectionDamper, _RimPower, _RimIntensity;
			float4 _MainTex_ST;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 scrpos : TEXCOORD1;
				float fresnel : TEXCOORD2;
			};
			v2f vert (appdata_base v)
			{
				float4 wp = mul(unity_ObjectToWorld, v.vertex);
				float3 vdir = normalize(ObjSpaceViewDir(v.vertex));

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.scrpos = ComputeNonStereoScreenPos(o.pos);
				o.scrpos.z = lerp(o.pos.w, mul(UNITY_MATRIX_V, wp).z, unity_OrthoParams.w);
				o.fresnel = 1.0 - saturate(dot(vdir, v.normal));
				return o;
			}
			half4 frag (v2f input, fixed facing : VFACE) : SV_Target
			{
				float sceneZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.scrpos));
				float perpectiveZ = LinearEyeDepth(sceneZ);
#if defined(UNITY_REVERSED_Z)
				sceneZ = 1 - sceneZ;
#endif
				float orthoZ = sceneZ * (_ProjectionParams.y - _ProjectionParams.z) - _ProjectionParams.y;
				sceneZ = lerp(perpectiveZ, orthoZ, unity_OrthoParams.w);
				float dist = sqrt(pow(sceneZ - input.scrpos.z, 2));

				half mask = max(0, sign(_IntersectionMax - dist));
				mask *= 1.0 - dist / _IntersectionMax * _IntersectionDamper;
				mask = saturate(mask);

				half4 col = tex2D(_MainTex, input.uv);
				col *= _MainColor * (1.0 - mask);
				col += _IntersectionColor * mask;

				float f = facing > 0 ? pow(input.fresnel, _RimPower) * _RimIntensity : 1.0;
				col.a *= f;

				col.a = max(mask, col.a);
				return col;
			}
			ENDCG
		}
	}
	FallBack "Unlit/Color"
}
