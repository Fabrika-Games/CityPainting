// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "UVFree/StandardMetallic/Top-Bottom-Texture" {
	Properties {
		// Triplanar space, for UI
		[HideInInspector] _TriplanarSpace("Triplanar Space", Float) = 0.0

		_TexPower("Texture Power", Range(0, 20)) = 10.0
		_TopMultiplier ("Top Multiplier", Range(0,8)) = 1.0
		_BottomMultiplier ("Bottom Multiplier", Range(0,8)) = 0.0
		_VertexColorStrength("Vertex Color Strength", Range(0,1)) = 1.0

		// FRONT
		
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		[Gamma] _Metallic ("Metallic", Range(0,1)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "black" {}
		_UsingMetallicGlossMap("Using Metallic Gloss Map", float) = 0.0
		
		_BumpScale("Bump Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_EmissionColor("Emission Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		[HideInInspector] _EmissionScaleUI("Scale", Float) = 0.0
		[HideInInspector] _EmissionColorUI("Color", Color) = (1,1,1)
		
		// TOP
		
		_TopColor ("Color", Color) = (1,1,1,1)
		_TopMainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_TopGlossiness ("Smoothness", Range(0,1)) = 0.0
		[Gamma] _TopMetallic ("Metallic", Range(0,1)) = 0.0
		_TopMetallicGlossMap("Metallic", 2D) = "black" {}
		_TopUsingMetallicGlossMap("Using Metallic Gloss Map", float) = 0.0
		
		_TopBumpScale("Bump Scale", Float) = 1.0
		_TopBumpMap("Normal Map", 2D) = "bump" {}

		_TopEmissionColor("Emission Color", Color) = (0,0,0)
		_TopEmissionMap("Emission", 2D) = "white" {}

		[HideInInspector] _TopEmissionScaleUI("Scale", Float) = 0.0
		[HideInInspector] _TopEmissionColorUI("Color", Color) = (1,1,1)

		// BOTTOM
		// bottom is ignored unless _UVFREE_BOTTOM shader feature is turned on
		
		_BottomColor ("Color", Color) = (1,1,1,1)
		_BottomMainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_BottomGlossiness ("Smoothness", Range(0,1)) = 0.5
		[Gamma] _BottomMetallic ("Metallic", Range(0,1)) = 0.0
		_BottomMetallicGlossMap("Metallic", 2D) = "black" {}
		_BottomUsingMetallicGlossMap("Using Metallic Gloss Map", float) = 0.0
		
		_BottomBumpScale("Bump Scale", Float) = 1.0
		_BottomBumpMap("Normal Map", 2D) = "bump" {}

		_BottomEmissionColor("Emission Color", Color) = (0,0,0)
		_BottomEmissionMap("Emission", 2D) = "white" {}

		[HideInInspector] _BottomEmissionScaleUI("Scale", Float) = 0.0
		[HideInInspector] _BottomEmissionColorUI("Color", Color) = (1,1,1)
	}
	SubShader {
		Tags {
			"RenderType"="Opaque"
			"PerformanceChecks"="False"
			
			// Uncomment the following line if you are using the shader in
			// local mode, and are seeing the textures move when you zoom in
			// due to dynamic batching. (Dynamic batching converts local vertex
			// data into world space, making local vertex position data unavailable
			// to the shader.)
			
			// "DisableBatching"="True"

		}
	
		LOD 300

		CGPROGRAM
		#pragma target 3.0
		
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma shader_feature _UVFREE_LOCAL
		#pragma shader_feature _EMISSION
		#pragma shader_feature _METALLICGLOSSMAP
		#pragma shader_feature _UVFREE_BOTTOM

		// Uncomment to enable experimental feature which flips
		// backward textures. Note: Causes some normals to be flipped.
		// #define _UVFREE_FLIP_BACKWARD_TEXTURES
		
		// Comment out following line to omit vertex colors
		#define _UVFREE_VERTEX_COLOR
		
		half _TexPower;
		half _TopMultiplier;
		half _BottomMultiplier;
		
		// FRONT
		
		fixed4		_Color;

		sampler2D	_MainTex;
		float4		_MainTex_ST;

		sampler2D	_BumpMap;
		half		_BumpScale;

		sampler2D	_MetallicGlossMap;
		fixed		_Metallic;
		fixed		_Glossiness;
		fixed		_UsingMetallicGlossMap;
		
		half4 		_EmissionColor;
		sampler2D	_EmissionMap;
				
		// TOP
		
		fixed4		_TopColor;

		sampler2D	_TopMainTex;
		float4		_TopMainTex_ST;

		sampler2D	_TopBumpMap;
		half		_TopBumpScale;

		sampler2D	_TopMetallicGlossMap;
		half		_TopMetallic;
		half		_TopGlossiness;
		fixed		_TopUsingMetallicGlossMap;

		half4 		_TopEmissionColor;
		sampler2D	_TopEmissionMap;
				
		// BOTTOM
		
		fixed4		_BottomColor;

		sampler2D	_BottomMainTex;
		float4		_BottomMainTex_ST;

		sampler2D	_BottomBumpMap;
		half		_BottomBumpScale;

		sampler2D	_BottomMetallicGlossMap;
		half		_BottomMetallic;
		half		_BottomGlossiness;
		fixed		_BottomUsingMetallicGlossMap;

		half4 		_BottomEmissionColor;
		sampler2D	_BottomEmissionMap;
				
		#ifdef		_UVFREE_VERTEX_COLOR
		fixed		_VertexColorStrength;
		#endif
						
		struct Input {
			fixed3 powerNormal;
			fixed3 normal;
			
			float3 worldPos;
			
			#ifdef _UVFREE_VERTEX_COLOR
				fixed4 color:COLOR;
			#endif				
		};

		void vert (inout appdata_full v, out Input o) {
		
			UNITY_INITIALIZE_OUTPUT(Input,o);
						
			#ifdef _UVFREE_LOCAL
				o.normal = v.normal;
				fixed3 powerNormal = v.normal;
				fixed3 weightedPowerNormal = v.normal;
			#else	
				fixed3 worldNormal = normalize(mul(unity_ObjectToWorld, fixed4(v.normal, 0)).xyz);					
				o.normal = worldNormal;
				fixed3 powerNormal = worldNormal;
				fixed3 weightedPowerNormal = worldNormal;
				
			#endif	

			// texpower sets the sharpness
			powerNormal = pow(abs(powerNormal), _TexPower);
			powerNormal = max(powerNormal, 0.0001);
			powerNormal /= dot(powerNormal, 1.0);
			o.powerNormal = powerNormal;
			
			#if _UVFREE_BOTTOM
				weightedPowerNormal.y = max(0.0, weightedPowerNormal.y)*_TopMultiplier + min(0.0, weightedPowerNormal.y)*_BottomMultiplier;
			#else
				weightedPowerNormal.y = max(0.0, weightedPowerNormal.y)*_TopMultiplier + min(0.0, weightedPowerNormal.y);
			#endif
			
			weightedPowerNormal = pow(abs(weightedPowerNormal), _TexPower);
			weightedPowerNormal = max(weightedPowerNormal, 0.0001);
			weightedPowerNormal /= dot(weightedPowerNormal, 1.0);
						
			fixed3 lerpedPowerNormal = lerp(powerNormal, weightedPowerNormal, weightedPowerNormal.y);

			#ifdef _UVFREE_LOCAL
				
				v.tangent.xyz = 
					cross(v.normal, fixed3(0.0,sign(v.normal.x),0.0)) * (lerpedPowerNormal.x)
				  + cross(v.normal, fixed3(0.0,0.0,sign(v.normal.y))) * (lerpedPowerNormal.y)
				  + cross(v.normal, fixed3(0.0,sign(v.normal.z),0.0)) * (lerpedPowerNormal.z)
				;
				
				v.tangent.w = dot(-v.normal, lerpedPowerNormal);

			#else
				
				v.tangent.xyz = 
					cross(v.normal, mul(unity_WorldToObject,fixed4(0.0,sign(worldNormal.x),0.0,0.0)).xyz * (lerpedPowerNormal.x))
				  + cross(v.normal, mul(unity_WorldToObject,fixed4(0.0,0.0,sign(worldNormal.y),0.0)).xyz * (lerpedPowerNormal.y))
				  + cross(v.normal, mul(unity_WorldToObject,fixed4(0.0,sign(worldNormal.z),0.0,0.0)).xyz * (lerpedPowerNormal.z))
				;
				
				v.tangent.w = dot(-worldNormal, lerpedPowerNormal);

			#endif				
		}
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			fixed topLerp = smoothstep(0.0, 1.0, _TopMultiplier);
			
			#if _UVFREE_BOTTOM
				fixed bottomLerp = smoothstep(0.0, 1.0, _BottomMultiplier);
			#endif
			
			fixed3 weightedPowerNormal = IN.normal;
			#if _UVFREE_BOTTOM
				weightedPowerNormal.y = max(0.0, weightedPowerNormal.y)*_TopMultiplier + min(0.0, weightedPowerNormal.y)*_BottomMultiplier;
			#else
				weightedPowerNormal.y = max(0.0, weightedPowerNormal.y)*_TopMultiplier + min(0.0, weightedPowerNormal.y);
			#endif
			
			weightedPowerNormal = pow(abs(weightedPowerNormal), _TexPower);
			weightedPowerNormal = max(weightedPowerNormal, 0.0001);
			weightedPowerNormal /= dot(weightedPowerNormal, 1.0);

			fixed topBottomLerp = sign(IN.normal.y)*0.5 + 0.5;
			fixed yLerp = weightedPowerNormal.y;
			
			// TRIPLANAR UVs BASED ON WORLD OR LOCAL POSITION
			//
			
			#ifdef _UVFREE_LOCAL
				float3 pos = mul(unity_WorldToObject, float4(IN.worldPos, 1.0)).xyz;
				
				float2 posX = pos.zy;
				float2 posY = pos.xz;
				float2 posZ = float2(-pos.x, pos.y);
			#else
				float2 posX = IN.worldPos.zy;
				float2 posY = IN.worldPos.xz;
				float2 posZ = float2(-IN.worldPos.x, IN.worldPos.y);				
			#endif

			float2 xUV = posX * _MainTex_ST.xy + _MainTex_ST.zw;
			
			float2 yUVTop = posY * _TopMainTex_ST.xy + _TopMainTex_ST.zw;
			float2 yUVBottom = posY * _BottomMainTex_ST.xy + _BottomMainTex_ST.zw;
			float2 yUV = posY * _MainTex_ST.xy + _MainTex_ST.zw;
			
			float2 zUV = posZ * _MainTex_ST.xy + _MainTex_ST.zw;
			
			#ifdef _UVFREE_FLIP_BACKWARD_TEXTURES
				fixed3 powerSign = sign(IN.normal);
				xUV.x *= powerSign.x;
				zUV.x *= powerSign.z;
				yUV.y *= powerSign.y;
				yUVTop.y *= powerSign.y;
				yUVBottom.y *= powerSign.y;
			#endif
						
			// DIFFUSE
			//
			
			fixed4 texX = tex2D(_MainTex, xUV) * _Color;
			fixed4 texY = tex2D(_MainTex, yUV) * _Color;
			fixed4 texZ = tex2D(_MainTex, zUV) * _Color;
			
			fixed4 texTop = tex2D(_TopMainTex, yUVTop) * _TopColor;
			texTop = lerp(texY, texTop, topLerp);

			#if _UVFREE_BOTTOM					
				fixed4 texBottom = tex2D(_BottomMainTex, yUVBottom) * _BottomColor;
				texBottom = lerp(texY, texBottom, bottomLerp);
				texTop = lerp(texBottom, texTop, topBottomLerp);
			#else
				texTop = lerp(texY, texTop, topBottomLerp);
			#endif
			
			fixed4 tex = 
			    lerp(texX * IN.powerNormal.x, texTop * weightedPowerNormal.x, yLerp)
			  + lerp(texY * IN.powerNormal.y, texTop * weightedPowerNormal.y, yLerp)
			  + lerp(texZ * IN.powerNormal.z, texTop * weightedPowerNormal.z, yLerp)
			;
			
			#ifdef _UVFREE_VERTEX_COLOR			 
				tex *= lerp(fixed4(1.0,1.0,1.0,1.0), IN.color, _VertexColorStrength);
			#endif
				
			o.Albedo = max(fixed3(0.001, 0.001, 0.001), tex.rgb);
			o.Alpha = tex.a;
			
			// NORMAL
			//

			fixed3 bumpX = UnpackScaleNormal(tex2D(_BumpMap, xUV), _BumpScale);
			fixed3 bumpY = UnpackScaleNormal(tex2D(_BumpMap, yUV), _BumpScale);
			fixed3 bumpZ = UnpackScaleNormal(tex2D(_BumpMap, zUV), _BumpScale);
			
			fixed3 bumpTop = UnpackScaleNormal(tex2D(_TopBumpMap, yUVTop), _TopBumpScale);
			bumpTop = lerp(bumpY, bumpTop, topLerp);

			#if _UVFREE_BOTTOM					
				fixed3 bumpBottom = UnpackScaleNormal(tex2D(_BottomBumpMap, yUVBottom), _BottomBumpScale);			
				bumpBottom = lerp(bumpY, bumpBottom, bottomLerp);
				bumpTop = lerp(bumpBottom, bumpTop, topBottomLerp);
			#else
				bumpTop = lerp(bumpY, bumpTop, topBottomLerp);
			#endif

			o.Normal = normalize( 
			    lerp(bumpX * IN.powerNormal.x, bumpTop * weightedPowerNormal.x, yLerp)
			  + lerp(bumpY * IN.powerNormal.y, bumpTop * weightedPowerNormal.y, yLerp)
			  + lerp(bumpZ * IN.powerNormal.z, bumpTop * weightedPowerNormal.z, yLerp)
			  );
						
			// METALLIC/GLOSS
			//
				
			fixed2 mgX = fixed2(_Metallic, _Glossiness);
			fixed2 mgY = mgX;
			fixed2 mgZ = mgX;
			fixed2 mgTop = fixed2(_TopMetallic, _TopGlossiness);
			
			#ifdef _UVFREE_BOTTOM
				fixed2 mgBottom = fixed2(_BottomMetallic, _BottomGlossiness);				
			#endif
			
			#ifdef _METALLICGLOSSMAP
				
				mgX = lerp(mgX, tex2D(_MetallicGlossMap, xUV).ra, _UsingMetallicGlossMap);
				mgY = lerp(mgY, tex2D(_MetallicGlossMap, yUV).ra, _UsingMetallicGlossMap);
				mgZ = lerp(mgZ, tex2D(_MetallicGlossMap, zUV).ra, _UsingMetallicGlossMap);
				
				mgTop = lerp(mgTop, tex2D(_TopMetallicGlossMap, yUVTop).ra, _TopUsingMetallicGlossMap);
				#ifdef _UVFREE_BOTTOM
					mgBottom = lerp(mgBottom, tex2D(_BottomMetallicGlossMap, yUVBottom).ra, _BottomUsingMetallicGlossMap);
				#endif

			#endif
			
			mgTop = lerp(mgY, mgTop, topLerp);
			
			#ifdef _UVFREE_BOTTOM			
				mgBottom = lerp(mgY, mgBottom, bottomLerp);
				mgTop = lerp(mgBottom, mgTop, topBottomLerp);
			#else
				mgTop = lerp(mgY, mgTop, topBottomLerp);
			#endif
				
			fixed2 mg = 
			    lerp(mgX * IN.powerNormal.x, mgTop * weightedPowerNormal.x, yLerp)
			  + lerp(mgY * IN.powerNormal.y, mgTop * weightedPowerNormal.y, yLerp)
			  + lerp(mgZ * IN.powerNormal.z, mgTop * weightedPowerNormal.z, yLerp)
			;

			o.Metallic = mg.x;
			o.Smoothness = mg.y;
			
			// EMISSION
			//
				
			#ifdef _EMISSION
				half3 emissionX = tex2D(_EmissionMap, xUV).rgb * _EmissionColor.rgb;
				half3 emissionY = tex2D(_EmissionMap, yUV).rgb * _EmissionColor.rgb;
				
				half3 emissionYTop = tex2D(_TopEmissionMap, yUVTop).rgb * _TopEmissionColor.rgb;
				emissionYTop = lerp(emissionY, emissionYTop, topLerp);

				#ifdef _UVFREE_BOTTOM			
					half3 emissionYBottom = tex2D(_BottomEmissionMap, yUVBottom).rgb * _BottomEmissionColor.rgb;
					emissionYBottom = lerp(emissionY, emissionYBottom, bottomLerp);
					emissionYTop = lerp(emissionYBottom, emissionYTop, topBottomLerp);
				#else
					emissionYTop = lerp(emissionY, emissionYTop, topBottomLerp);				
				#endif
				
				half3 emissionZ = tex2D(_EmissionMap, zUV).rgb * _EmissionColor.rgb;
				
				o.Emission +=  
				    lerp(emissionX * IN.powerNormal.x, emissionYTop * weightedPowerNormal.x, yLerp)
				  + lerp(emissionY * IN.powerNormal.y, emissionYTop * weightedPowerNormal.y, yLerp)
				  + lerp(emissionZ * IN.powerNormal.z, emissionYTop * weightedPowerNormal.z, yLerp)
				;
			#endif
		
		}
		ENDCG
	} 
	FallBack "Diffuse"
	CustomEditor "UVFreePBRShaderTopBottomGUI"
}

