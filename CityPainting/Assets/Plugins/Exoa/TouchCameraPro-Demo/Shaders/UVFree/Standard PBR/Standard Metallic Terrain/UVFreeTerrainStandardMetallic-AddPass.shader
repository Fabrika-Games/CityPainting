// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/UVFree/Terrain/StandardMetallic-AddPass" {
	Properties {
		_TexPower("Texture Power", Range (0.0, 20.0)) = 10.0
		_UVScale("Texture Scale %", Float) = 100.0	
		_BumpScale("Bump Scale", Range(-2.0, 2.0)) = 1.0
		
		// set by terrain engine
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		[HideInInspector] [Gamma] _Metallic0 ("Metallic 0", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic1 ("Metallic 1", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic2 ("Metallic 2", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic3 ("Metallic 3", Range(0.0, 1.0)) = 0.0
		[HideInInspector] _Smoothness0 ("Smoothness 0", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness1 ("Smoothness 1", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness2 ("Smoothness 2", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness3 ("Smoothness 3", Range(0.0, 1.0)) = 1.0
	}

	SubShader {
		Tags {
			"SplatCount" = "4"
			"Queue" = "Geometry-99"
			"IgnoreProjector"="True"
			"RenderType" = "Opaque"
		}

		CGPROGRAM
		#pragma surface surf Standard decal:add vertex:SplatmapVert  finalcolor:SplatmapFinalColor finalprepass:SplatmapFinalPrepass finalgbuffer:SplatmapFinalGBuffer fullforwardshadows
		#define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandard

		#pragma multi_compile_fog
		#pragma target 3.0
		// needs more than 8 texcoords
		#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"

		#pragma multi_compile __ _TERRAIN_NORMAL_MAP
		#define TERRAIN_SPLAT_ADDPASS

		// Uncomment to enable experimental feature which flips
		// backward textures. Note: Causes some normals to be flipped.
		// #define _UVFREE_FLIP_BACKWARD_TEXTURES
		
		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
		half4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
		
		#ifdef _TERRAIN_NORMAL_MAP
			sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
		#endif

		fixed _Metallic0, _Metallic1, _Metallic2, _Metallic3;
		fixed _Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3;
		
		half _TexPower;
		half _BumpScale;
		
		float _UVScale;

		struct Input
		{
			fixed3 powerNormal;
			#ifdef _UVFREE_FLIP_BACKWARD_TEXTURES
				fixed3 normal;
			#endif
			float3 worldPos;			
			float2 tc_Control : TEXCOORD4;	// Not prefixing '_Control' with 'uv' allows a tighter packing of interpolators, which is necessary to support directional lightmap.
			UNITY_FOG_COORDS(5)
		};
		
		void SplatmapVert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
		
			fixed3 worldNormal = normalize(mul(unity_ObjectToWorld, fixed4(v.normal, 0)).xyz);
			#ifdef _UVFREE_FLIP_BACKWARD_TEXTURES
					o.normal = worldNormal;
			#endif
			fixed3 powerNormal = pow(abs(worldNormal), clamp(_TexPower,0.0, 20.0));
			
			powerNormal = max(powerNormal, 0.0001);
			powerNormal /= dot(powerNormal, half3(1.0,1.0,1.0));
			
			o.powerNormal = powerNormal;

			v.tangent.xyz = 
				cross(v.normal, mul(unity_WorldToObject,fixed4(0.0,sign(worldNormal.x),0.0,0.0)).xyz * (powerNormal.x))
			  + cross(v.normal, mul(unity_WorldToObject,fixed4(0.0,0.0,sign(worldNormal.y),0.0)).xyz * (powerNormal.y))
			  + cross(v.normal, mul(unity_WorldToObject,fixed4(0.0,sign(worldNormal.z),0.0,0.0)).xyz * (powerNormal.z))
			;
		
			v.tangent.w = 
				(-(worldNormal.x) * (powerNormal.x))
			  +	(-(worldNormal.y) * (powerNormal.y))
			  +	(-(worldNormal.z) * (powerNormal.z))
			;

			// Need to manually transform uv here,
			// as we choose not to use 'uv' prefix for this texcoord.			
			o.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);
			float4 pos = UnityObjectToClipPos (v.vertex);
			UNITY_TRANSFER_FOG(o, pos);			
		}
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
		
			// world space positions along the axis planes
			float2 posX = IN.worldPos.zy;
			float2 posY = IN.worldPos.xz;
			float2 posZ = float2(-IN.worldPos.x, IN.worldPos.y);
			
			#ifdef _UVFREE_FLIP_BACKWARD_TEXTURES
				fixed3 powerSign = sign(IN.normal);
				float2 xUV = float2(powerSign.x, 1.0);
				float2 zUV = float2(powerSign.z, 1.0);
				float2 yUV = float2(1.0, powerSign.y);
			#else
				float2 xUV = float2(1.0, 1.0);
				float2 zUV = xUV;
				float2 yUV = xUV;
			#endif

							
			half4 splat_control;
			half weight;
			fixed4 mixedDiffuse;
						
			splat_control = tex2D(_Control, IN.tc_Control);
			weight = dot(splat_control, half4(1.0,1.0,1.0,1.0));

			#ifndef UNITY_PASS_DEFERRED
				splat_control /= (weight + 1e-3f); // avoid NaNs in splat_control
			#endif

			#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(weight - 0.0039);
			#endif

			fixed4 texX0 = tex2D(_Splat0, (posX * _Splat0_ST.xy + _Splat0_ST.zw)*xUV/_UVScale);
			fixed4 texY0 = tex2D(_Splat0, (posY * _Splat0_ST.xy + _Splat0_ST.zw)*yUV/_UVScale);
			fixed4 texZ0 = tex2D(_Splat0, (posZ * _Splat0_ST.xy + _Splat0_ST.zw)*zUV/_UVScale);

			fixed4 texX1 = tex2D(_Splat1, (posX * _Splat1_ST.xy + _Splat1_ST.zw)*xUV/_UVScale);
			fixed4 texY1 = tex2D(_Splat1, (posY * _Splat1_ST.xy + _Splat1_ST.zw)*yUV/_UVScale);
			fixed4 texZ1 = tex2D(_Splat1, (posZ * _Splat1_ST.xy + _Splat1_ST.zw)*zUV/_UVScale);

			fixed4 texX2 = tex2D(_Splat2, (posX * _Splat2_ST.xy + _Splat2_ST.zw)*xUV/_UVScale);
			fixed4 texY2 = tex2D(_Splat2, (posY * _Splat2_ST.xy + _Splat2_ST.zw)*yUV/_UVScale);
			fixed4 texZ2 = tex2D(_Splat2, (posZ * _Splat2_ST.xy + _Splat2_ST.zw)*zUV/_UVScale);

			fixed4 texX3 = tex2D(_Splat3, (posX * _Splat3_ST.xy + _Splat3_ST.zw)*xUV/_UVScale);
			fixed4 texY3 = tex2D(_Splat3, (posY * _Splat3_ST.xy + _Splat3_ST.zw)*yUV/_UVScale);
			fixed4 texZ3 = tex2D(_Splat3, (posZ * _Splat3_ST.xy + _Splat3_ST.zw)*zUV/_UVScale);
												
			fixed4 tex0 = 
			  + texX0 * IN.powerNormal.x
			  + texY0 * IN.powerNormal.y
			  + texZ0 * IN.powerNormal.z;
			  
			fixed4 tex1 = 
			  + texX1 * IN.powerNormal.x
			  + texY1 * IN.powerNormal.y
			  + texZ1 * IN.powerNormal.z;

			fixed4 tex2 = 
			  + texX2 * IN.powerNormal.x
			  + texY2 * IN.powerNormal.y
			  + texZ2 * IN.powerNormal.z;

			fixed4 tex3 = 
			  + texX3 * IN.powerNormal.x
			  + texY3 * IN.powerNormal.y
			  + texZ3 * IN.powerNormal.z;

			mixedDiffuse = 0.0f;
			mixedDiffuse += splat_control.r * tex0 * fixed4(1.0, 1.0, 1.0, _Smoothness0);
			mixedDiffuse += splat_control.g * tex1 * fixed4(1.0, 1.0, 1.0, _Smoothness1);
			mixedDiffuse += splat_control.b * tex2 * fixed4(1.0, 1.0, 1.0, _Smoothness2);
			mixedDiffuse += splat_control.a * tex3 * fixed4(1.0, 1.0, 1.0, _Smoothness3);

			#ifdef _TERRAIN_NORMAL_MAP
			
				// NORMAL
				//
				fixed4 flatNormal = fixed4(0.5,0.5,1.0,0.5); // this is "flat normal" in both DXT5nm and xyz*2-1 cases
				
				fixed4 bumpX0 = tex2D(_Normal0, (posX * _Splat0_ST.xy + _Splat0_ST.zw)*xUV/_UVScale) - flatNormal;
				fixed4 bumpY0 = tex2D(_Normal0, (posY * _Splat0_ST.xy + _Splat0_ST.zw)*yUV/_UVScale) - flatNormal;
				fixed4 bumpZ0 = tex2D(_Normal0, (posZ * _Splat0_ST.xy + _Splat0_ST.zw)*zUV/_UVScale) - flatNormal;
				
				fixed4 bumpX1 = tex2D(_Normal1, (posX * _Splat1_ST.xy + _Splat1_ST.zw)*xUV/_UVScale) - flatNormal;
				fixed4 bumpY1 = tex2D(_Normal1, (posY * _Splat1_ST.xy + _Splat1_ST.zw)*yUV/_UVScale) - flatNormal;
				fixed4 bumpZ1 = tex2D(_Normal1, (posZ * _Splat1_ST.xy + _Splat1_ST.zw)*zUV/_UVScale) - flatNormal;

				fixed4 bumpX2 = tex2D(_Normal2, (posX * _Splat2_ST.xy + _Splat2_ST.zw)*xUV/_UVScale) - flatNormal;
				fixed4 bumpY2 = tex2D(_Normal2, (posY * _Splat2_ST.xy + _Splat2_ST.zw)*yUV/_UVScale) - flatNormal;
				fixed4 bumpZ2 = tex2D(_Normal2, (posZ * _Splat2_ST.xy + _Splat2_ST.zw)*zUV/_UVScale) - flatNormal;

				fixed4 bumpX3 = tex2D(_Normal3, (posX * _Splat3_ST.xy + _Splat3_ST.zw)*xUV/_UVScale) - flatNormal;
				fixed4 bumpY3 = tex2D(_Normal3, (posY * _Splat3_ST.xy + _Splat3_ST.zw)*yUV/_UVScale) - flatNormal;
				fixed4 bumpZ3 = tex2D(_Normal3, (posZ * _Splat3_ST.xy + _Splat3_ST.zw)*zUV/_UVScale) - flatNormal;

				fixed4 bump0 = 
				  + bumpX0 * IN.powerNormal.x
				  + bumpY0 * IN.powerNormal.y
				  + bumpZ0 * IN.powerNormal.z;
				 
				fixed4 bump1 = 
				  + bumpX1 * IN.powerNormal.x
				  + bumpY1 * IN.powerNormal.y
				  + bumpZ1 * IN.powerNormal.z;
				
				fixed4 bump2 = 
				  + bumpX2 * IN.powerNormal.x
				  + bumpY2 * IN.powerNormal.y
				  + bumpZ2 * IN.powerNormal.z;
				
				fixed4 bump3 = 
				  + bumpX3 * IN.powerNormal.x
				  + bumpY3 * IN.powerNormal.y
				  + bumpZ3 * IN.powerNormal.z;
				  			 
				fixed4 bump =
					splat_control.r * bump0
				  + splat_control.g * bump1
				  + splat_control.b * bump2
				  + splat_control.a * bump3;
				bump *= _BumpScale;
				
				bump += flatNormal;

				o.Normal = UnpackNormal(bump);
			
			#endif
			
			o.Albedo = max(fixed3(0.001, 0.001, 0.001), mixedDiffuse.rgb);
			o.Alpha = weight;
			
			o.Smoothness = mixedDiffuse.a;
			o.Metallic = dot(splat_control, fixed4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));
		}

		void SplatmapFinalColor(Input IN, TERRAIN_SURFACE_OUTPUT o, inout fixed4 color)
		{
			color *= o.Alpha;
			#ifdef TERRAIN_SPLAT_ADDPASS
				UNITY_APPLY_FOG_COLOR(IN.fogCoord, color, fixed4(0,0,0,0));
			#else
				UNITY_APPLY_FOG(IN.fogCoord, color);
			#endif
		}

		void SplatmapFinalPrepass(Input IN, TERRAIN_SURFACE_OUTPUT o, inout fixed4 normalSpec)
		{
			normalSpec *= o.Alpha;
		}

		void SplatmapFinalGBuffer(Input IN, TERRAIN_SURFACE_OUTPUT o, inout half4 diffuse, inout half4 specSmoothness, inout half4 normal, inout half4 emission)
		{
			diffuse.rgb *= o.Alpha;
			specSmoothness *= o.Alpha;
			normal.rgb *= o.Alpha;
			emission *= o.Alpha;
		}		


		ENDCG
	}


	Fallback "Hidden/TerrainEngine/Splatmap/Diffuse-AddPass"
}
