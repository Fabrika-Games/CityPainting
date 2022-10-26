Shader "Hidden/UVFree/Terrain/StandardSpecular-Base" {
	Properties {
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1.0)		
		_Smoothness ("Smoothness", Range (0.03, 1)) = 0.078125
		
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}

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
		#pragma surface surf StandardSpecular fullforwardshadows
		#pragma target 3.0
		#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"
		
		sampler2D _MainTex;
		fixed _Smoothness;
		
		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = max(fixed3(0.001, 0.001, 0.001),c.rgb);
			o.Alpha = 1.0;
			
			#ifdef _TERRAIN_OVERRIDE_SMOOTHNESS
				o.Smoothness = _Smoothness;				
			#else
				o.Smoothness = c.a;	
			#endif			

			o.Specular = _SpecColor;
		}

		ENDCG
	}

	FallBack "Diffuse"
}
