// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "UVFree/StandardSpecular/Top-Bottom-Texture" {
	Properties {
		// Triplanar space, for UI
		[HideInInspector] _TriplanarSpace("Triplanar Space", Float) = 0.0

		_TexPower("Texture Power", Range(0.0, 20.0)) = 10.0

		_TopMultiplier ("Top Multiplier", Range(0.0,8.0)) = 1.0
		_BottomMultiplier ("Bottom Multiplier", Range(0.0,8.0)) = 0.0
		_VertexColorStrength("Vertex Color Strength", Range(0.0,1.0)) = 1.0

		// FRONT
		
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_SpecColor("Specular", Color) = (0.2,0.2,0.2)
		_SpecGlossMap("Specular", 2D) = "white" {}		
		
		_UsingSpecGlossMap("Using Spec Gloss Map", float) = 0.0
		
		_BumpScale("Bump Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_EmissionColor("Emission Color", Color) = (0.0,0.0,0.0)
		_EmissionMap("Emission", 2D) = "white" {}

		[HideInInspector] _EmissionScaleUI("Scale", Float) = 0.0
		[HideInInspector] _EmissionColorUI("Color", Color) = (1.0,1.0,1.0)
		
		// TOP
		
		_TopColor ("Color", Color) = (1.0,1.0,1.0,1.0)
		_TopMainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_TopGlossiness("Smoothness", Range(0.0, 1.0)) = 0.5		
		_TopSpecColor("Specular", Color) = (0.2,0.2,0.2)
		_TopSpecGlossMap("Specular", 2D) = "white" {}		
		
		_TopUsingSpecGlossMap("Using Spec Gloss Map", float) = 0.0

		_TopBumpScale("Bump Scale", Float) = 1.0
		_TopBumpMap("Normal Map", 2D) = "bump" {}

		_TopEmissionColor("Emission Color", Color) = (0.0,0.0,0.0)
		_TopEmissionMap("Emission", 2D) = "white" {}

		[HideInInspector] _TopEmissionScaleUI("Scale", Float) = 0.0
		[HideInInspector] _TopEmissionColorUI("Color", Color) = (1,1,1)

		// BOTTOM
		// bottom is ignored unless _UVFREE_BOTTOM shader feature is turned on
		
		_BottomColor ("Color", Color) = (1.0,1.0,1.0,1.0)
		_BottomMainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_BottomGlossiness("Smoothness", Range(0.0, 1.0)) = 0.5		
		_BottomSpecColor("Specular", Color) = (0.2,0.2,0.2)
		_BottomSpecGlossMap("Specular", 2D) = "white" {}		
		
		_BottomUsingSpecGlossMap("Using Spec Gloss Map", float) = 0.0

		_BottomBumpScale("Bump Scale", Float) = 1.0
		_BottomBumpMap("Normal Map", 2D) = "bump" {}

		_BottomEmissionColor("Emission Color", Color) = (0.0,0.0,0.0)
		_BottomEmissionMap("Emission", 2D) = "white" {}

		[HideInInspector] _BottomEmissionScaleUI("Scale", Float) = 0.0
		[HideInInspector] _BottomEmissionColorUI("Color", Color) = (1.0,1.0,1.0)
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
		#pragma surface surf StandardSpecular vertex:vert fullforwardshadows
		#pragma shader_feature _UVFREE_LOCAL
		#pragma shader_feature _EMISSION
		#pragma shader_feature _SPECGLOSSMAP
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

		sampler2D	_SpecGlossMap;		
		fixed		_Glossiness;
		fixed		_UsingSpecGlossMap;
		
		half4 		_EmissionColor;
		sampler2D	_EmissionMap;
				
		// TOP
		
		fixed4		_TopColor;

		sampler2D	_TopMainTex;
		float4		_TopMainTex_ST;

		sampler2D	_TopBumpMap;
		half		_TopBumpScale;

		fixed4		_TopSpecColor;
		sampler2D	_TopSpecGlossMap;		
		fixed		_TopGlossiness;
		fixed		_TopUsingSpecGlossMap;

		half4 		_TopEmissionColor;
		sampler2D	_TopEmissionMap;
				
		// BOTTOM
		
		fixed4		_BottomColor;

		sampler2D	_BottomMainTex;
		float4		_BottomMainTex_ST;

		sampler2D	_BottomBumpMap;
		half		_BottomBumpScale;
		
		fixed4		_BottomSpecColor;
		sampler2D	_BottomSpecGlossMap;		
		fixed		_BottomGlossiness;
		fixed		_BottomUsingSpecGlossMap;

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
				fixed3 powerNormal = v.normal;
				o.normal = v.normal;
				fixed topFactor = sign(v.normal.y)*0.5+0.5;
				fixed3 weightedPowerNormal = v.normal;
				
			#else
				fixed3 worldNormal = normalize(mul(unity_ObjectToWorld, fixed4(v.normal, 0.0)).xyz);
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
		
		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			
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
			
			o.Albedo = max(fixed3(0.001, 0.001, 0.001),tex.rgb);
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
												
			// SPECULAR/GLOSS
			//
			
			fixed4 sgX = fixed4(_SpecColor.rgb, _Glossiness);
			fixed4 sgY = sgX;
			fixed4 sgZ = sgX;
			fixed4 sgTop = fixed4(_TopSpecColor.rgb, _TopGlossiness);
			
			#ifdef _UVFREE_BOTTOM
				fixed4 sgBottom = fixed4(_BottomSpecColor.rgb, _BottomGlossiness);
			#endif
			
			#ifdef _SPECGLOSSMAP
				
				sgX = lerp(sgX, tex2D(_SpecGlossMap, xUV), _UsingSpecGlossMap);
				sgY = lerp(sgY, tex2D(_SpecGlossMap, yUV), _UsingSpecGlossMap);
				sgZ = lerp(sgZ, tex2D(_SpecGlossMap, zUV), _UsingSpecGlossMap);
				
				sgTop = lerp(sgTop, tex2D(_TopSpecGlossMap, yUVTop), _TopUsingSpecGlossMap);
				#ifdef _UVFREE_BOTTOM
					sgBottom = lerp(sgBottom, tex2D(_BottomSpecGlossMap, yUVBottom), _BottomUsingSpecGlossMap);
				#endif

			#endif
			
			sgTop = lerp(sgY, sgTop, topLerp);
			
			#ifdef _UVFREE_BOTTOM			
				sgBottom = lerp(sgY, sgBottom, bottomLerp);
				sgTop = lerp(sgBottom, sgTop, topBottomLerp);
			#else
				sgTop = lerp(sgY, sgTop, topBottomLerp);
			#endif
				
			fixed4 sg = 
			    lerp(sgX * IN.powerNormal.x, sgTop * weightedPowerNormal.x, yLerp)
			  + lerp(sgY * IN.powerNormal.y, sgTop * weightedPowerNormal.y, yLerp)
			  + lerp(sgZ * IN.powerNormal.z, sgTop * weightedPowerNormal.z, yLerp)
			;

			o.Specular = sg.rgb;
			o.Smoothness = sg.a;
			
			
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
