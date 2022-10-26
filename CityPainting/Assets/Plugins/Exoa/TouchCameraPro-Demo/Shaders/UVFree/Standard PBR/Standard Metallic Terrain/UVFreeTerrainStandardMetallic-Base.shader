Shader "Hidden/UVFree/Terrain/StandardMetallic-Base" {
	Properties {
		_MainTex ("Base (RGB) Smoothness (A)", 2D) = "white" {}
		_SpecularMetallicTex ("Specular (RGB) Metallic (A)", 2D) = "white" {}
		
		// used in fallback on old cards
		_Color ("Main Color", Color) = (1.0,1.0,1.0,1.0)
	}

	SubShader {
		Tags {
			"RenderType" = "Opaque"
			"Queue" = "Geometry-100"
		}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows		
		#pragma target 3.0
		// needs more than 8 texcoords
		#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"

		sampler2D _MainTex;
		sampler2D _SpecularMetallicTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = max(fixed3(0.001, 0.001, 0.001), c.rgb);
			o.Alpha = 1.0;

			o.Smoothness = c.a;
			o.Metallic = tex2D (_SpecularMetallicTex, IN.uv_MainTex).a;
		}

		ENDCG
	}

	FallBack "Diffuse"
}
