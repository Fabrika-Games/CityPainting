using System;
using UnityEngine;

namespace UnityEditor
{
	internal class UVFreePBRShaderGUI : ShaderGUI
	{
		private enum WorkflowMode
		{
			Specular,
			Metallic,
			Dielectric
		}

		private static class Styles
		{
			public static GUIStyle optionsButton = "PaneOptions";

			public static string emptyTootip = "";
			public static GUIContent triplanarSpace = new GUIContent("Triplanar Space", "Whether the triplanar projection should be in local or global space");
			public static GUIContent texturePowerText = new GUIContent("Texture Power", "How strong textures on different axes are differentiated. 1 for smooth blending, 10+ for sharper contrasts");
			public static GUIContent vertexColorStrengthText = new GUIContent("Vertex Color Strength", "0 for no vertex color (off), 1 for full vertex color tinting");

			public static GUIContent albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
			public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");
			public static GUIContent specularMapText = new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");
			public static GUIContent metallicMapText = new GUIContent("Metallic", "Metallic (R) and Smoothness (A)");
			public static GUIContent smoothnessText = new GUIContent("Smoothness", "");
			public static GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map");
			public static GUIContent heightMapText = new GUIContent("Height Map", "Height Map (G)");
			public static GUIContent occlusionText = new GUIContent("Occlusion", "Occlusion (G)");
			public static GUIContent emissionText = new GUIContent("Emission", "Emission (RGB)");
			public static GUIContent detailMaskText = new GUIContent("Detail Mask", "Mask for Secondary Maps (A)");
			public static GUIContent detailAlbedoText = new GUIContent("Detail Albedo x2", "Albedo (RGB) multiplied by 2");
			public static GUIContent detailNormalMapText = new GUIContent("Normal Map", "Normal Map");
			
			public static string whiteSpaceString = " ";
			public static string primaryMapsText = "Main Maps";
			public static string secondaryMapsText = "Secondary Maps";
			public static GUIContent emissiveWarning = new GUIContent ("Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");
			public static GUIContent emissiveColorWarning = new GUIContent ("Ensure emissive color is non-black for emission to have effect.");
			public static readonly string[] spaceNames = Enum.GetNames (typeof(Space));
		}
		MaterialProperty vertexColorStrength = null;
		MaterialProperty albedoMap = null;
		MaterialProperty albedoColor = null;
		MaterialProperty specularMap = null;
		MaterialProperty specularColor = null;
		MaterialProperty metallicMap = null;
		MaterialProperty metallic = null;
		MaterialProperty smoothness = null;
		MaterialProperty bumpScale = null;
		MaterialProperty bumpMap = null;
		MaterialProperty occlusionStrength = null;
		MaterialProperty occlusionMap = null;
		MaterialProperty heigtMapScale = null;
		MaterialProperty heightMap = null;
		MaterialProperty emissionScaleUI = null;
		MaterialProperty emissionColorUI = null;
		MaterialProperty emissionColorForRendering = null;
		MaterialProperty emissionMap = null;
		MaterialProperty detailMask = null;
		MaterialProperty detailAlbedoMap = null;
		MaterialProperty detailNormalMapScale = null;
		MaterialProperty detailNormalMap = null;
		MaterialProperty texturePower = null;
		MaterialProperty triplanarSpace = null;
		MaterialEditor m_MaterialEditor;
		WorkflowMode m_WorkflowMode = WorkflowMode.Specular;
		
		bool m_FirstTimeApply = true;
		
		public void FindProperties (MaterialProperty[] props)
		{
			triplanarSpace = FindProperty("_TriplanarSpace", props, false);
			texturePower = FindProperty("_TexPower", props, false);

			vertexColorStrength = FindProperty("_VertexColorStrength", props, false);

			albedoMap = FindProperty ("_MainTex", props);
			albedoColor = FindProperty ("_Color", props);
			specularMap = FindProperty ("_SpecGlossMap", props, false);
			specularColor = FindProperty ("_SpecColor", props, false);
			metallicMap = FindProperty ("_MetallicGlossMap", props, false);
			metallic = FindProperty ("_Metallic", props, false);

			if (specularMap != null && specularColor != null)
				m_WorkflowMode = WorkflowMode.Specular;
			else if (metallicMap != null && metallic != null)
				m_WorkflowMode = WorkflowMode.Metallic;
			else
				m_WorkflowMode = WorkflowMode.Dielectric;

			smoothness = FindProperty ("_Glossiness", props);
			bumpScale = FindProperty ("_BumpScale", props);
			bumpMap = FindProperty ("_BumpMap", props);
			heigtMapScale = FindProperty ("_Parallax", props);
			heightMap = FindProperty("_ParallaxMap", props);
			occlusionStrength = FindProperty ("_OcclusionStrength", props);
			occlusionMap = FindProperty ("_OcclusionMap", props);
			emissionScaleUI = FindProperty ("_EmissionScaleUI", props);
			emissionColorUI = FindProperty ("_EmissionColorUI", props);
			emissionColorForRendering = FindProperty ("_EmissionColor", props);
			emissionMap = FindProperty ("_EmissionMap", props);
			detailMask = FindProperty ("_DetailMask", props);
			detailAlbedoMap = FindProperty ("_DetailAlbedoMap", props);
			detailNormalMapScale = FindProperty ("_DetailNormalMapScale", props);
			detailNormalMap = FindProperty ("_DetailNormalMap", props);
		}
		
		public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props)
		{
			FindProperties (props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
			m_MaterialEditor = materialEditor;
			Material material = materialEditor.target as Material;
			
			ShaderPropertiesGUI (material);
			
			// Make sure that needed keywords are set up if we're switching some existing
			// material to a standard shader.
			if (m_FirstTimeApply)
			{
				SetMaterialKeywords (material, m_WorkflowMode);
				m_FirstTimeApply = false;
			}
		}
		
		public void ShaderPropertiesGUI (Material material)
		{
			// Use default labelWidth
			EditorGUIUtility.labelWidth = 0f;
			
			// Detect any changes to the material
			EditorGUI.BeginChangeCheck();
			{
				TriplanarSpacePopup();
				m_MaterialEditor.RangeProperty (texturePower, "Texture Power");

				if (vertexColorStrength != null)
				{
					m_MaterialEditor.RangeProperty (vertexColorStrength, Styles.vertexColorStrengthText.text);
				}

				// Primary properties
				GUILayout.Label (Styles.primaryMapsText, EditorStyles.boldLabel);
				DoAlbedoArea(material);
				DoSpecularMetallicArea();
				m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
				m_MaterialEditor.TexturePropertySingleLine(Styles.heightMapText, heightMap, heightMap.textureValue != null ? heigtMapScale : null);
				m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap, occlusionMap.textureValue != null ? occlusionStrength : null);
				DoEmissionArea(material);
				m_MaterialEditor.TexturePropertySingleLine(Styles.detailMaskText, detailMask);
				EditorGUI.BeginChangeCheck();
				m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
				if (EditorGUI.EndChangeCheck())
					emissionMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake
				
				EditorGUILayout.Space();
				
				// Secondary properties
				GUILayout.Label(Styles.secondaryMapsText, EditorStyles.boldLabel);
				m_MaterialEditor.TexturePropertySingleLine(Styles.detailAlbedoText, detailAlbedoMap);
				m_MaterialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, detailNormalMap, detailNormalMapScale);
				m_MaterialEditor.TextureScaleOffsetProperty(detailAlbedoMap);
			}
			if (EditorGUI.EndChangeCheck())
			{
				MaterialChanged(material, m_WorkflowMode);
			}
		}
		
		//internal
		void DetermineWorkflow(MaterialProperty[] props)
		{
			if (FindProperty("_SpecGlossMap", props, false) != null && FindProperty("_SpecColor", props, false) != null)
				m_WorkflowMode = WorkflowMode.Specular;
			else if (FindProperty("_MetallicGlossMap", props, false) != null && FindProperty("_Metallic", props, false) != null)
				m_WorkflowMode = WorkflowMode.Metallic;
			else
				m_WorkflowMode = WorkflowMode.Dielectric;
		}
		
		public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
		{
			base.AssignNewShaderToMaterial(material, oldShader, newShader);
			
			return;
		}

		void TriplanarSpacePopup()
		{
			EditorGUI.showMixedValue = triplanarSpace.hasMixedValue;
			var space = (Space)triplanarSpace.floatValue;
			
			EditorGUI.BeginChangeCheck();
			space = (Space)EditorGUILayout.Popup(Styles.triplanarSpace.text, (int)space, Styles.spaceNames);

			if (EditorGUI.EndChangeCheck())
			{
				m_MaterialEditor.RegisterPropertyChangeUndo("Triplanar Space");
				triplanarSpace.floatValue = (float)space;
			}
			
			EditorGUI.showMixedValue = false;
		}

		void DoAlbedoArea(Material material)
		{
			m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);
		}
		
		void DoEmissionArea(Material material)
		{
			bool showEmissionColorAndGIControls = emissionScaleUI.floatValue > 0f;
			bool hadEmissionTexture = emissionMap.textureValue != null;
			
			// Do controls
			m_MaterialEditor.TexturePropertySingleLine(Styles.emissionText, emissionMap, showEmissionColorAndGIControls ? emissionColorUI : null, emissionScaleUI);
			
			// Set default emissionScaleUI if texture was assigned
			if (emissionMap.textureValue != null && !hadEmissionTexture && emissionScaleUI.floatValue <= 0f)
				emissionScaleUI.floatValue = 1.0f;
			
			// Dynamic Lightmapping mode
			if (showEmissionColorAndGIControls)
			{
				bool shouldEmissionBeEnabled = ShouldEmissionBeEnabled(EvalFinalEmissionColor(material));
				EditorGUI.BeginDisabledGroup(!shouldEmissionBeEnabled);
				
				m_MaterialEditor.LightmapEmissionProperty (MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
				
				EditorGUI.EndDisabledGroup();
			}
			
			if (!HasValidEmissiveKeyword(material))
			{
				EditorGUILayout.HelpBox(Styles.emissiveWarning.text, MessageType.Warning);
			}
			
		}
		
		void DoSpecularMetallicArea()
		{
			if (m_WorkflowMode == WorkflowMode.Specular)
			{
				if (specularMap.textureValue == null)
					m_MaterialEditor.TexturePropertyTwoLines(Styles.specularMapText, specularMap, specularColor, Styles.smoothnessText, smoothness);
				else
					m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, specularMap);
				
			}
			else if (m_WorkflowMode == WorkflowMode.Metallic)
			{
				if (metallicMap.textureValue == null)
					m_MaterialEditor.TexturePropertyTwoLines(Styles.metallicMapText, metallicMap, metallic, Styles.smoothnessText, smoothness);
				else
					m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicMap);
			}
		}
		
		// Calculate final HDR _EmissionColor (gamma space) from _EmissionColorUI (LDR, gamma) & _EmissionScaleUI (gamma)
		static Color EvalFinalEmissionColor(Material material)
		{
			return material.GetColor("_EmissionColorUI") * material.GetFloat("_EmissionScaleUI");
		}
		
		static bool ShouldEmissionBeEnabled (Color color)
		{
			return color.grayscale > (0.1f / 255.0f);
		}
		
		static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
		{
			// Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
			// (MaterialProperty value might come from renderer material property block)
			if (workflowMode == WorkflowMode.Specular)
				SetKeyword (material, "_SPECGLOSSMAP", material.GetTexture ("_SpecGlossMap"));
			else if (workflowMode == WorkflowMode.Metallic)
				SetKeyword (material, "_METALLICGLOSSMAP", material.GetTexture ("_MetallicGlossMap"));
			SetKeyword (material, "_PARALLAXMAP", material.GetTexture ("_ParallaxMap"));
			SetKeyword (material, "_DETAIL", material.GetTexture ("_DetailAlbedoMap") || material.GetTexture ("_DetailNormalMap"));
			SetKeyword (material, "_OCCLUSION", material.GetTexture ("_OcclusionMap"));
			bool shouldEmissionBeEnabled = ShouldEmissionBeEnabled (material.GetColor("_EmissionColor"));
			SetKeyword (material, "_EMISSION", shouldEmissionBeEnabled);

			// Triplanar texture space"
			SetKeyword(material, "_UVFREE_LOCAL", material.GetFloat ("_TriplanarSpace") == (float) Space.Self);

			// Setup lightmap emissive flags
			MaterialGlobalIlluminationFlags flags = material.globalIlluminationFlags;
			if ((flags & (MaterialGlobalIlluminationFlags.BakedEmissive | MaterialGlobalIlluminationFlags.RealtimeEmissive)) != 0)
			{
				flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
				if (!shouldEmissionBeEnabled)
					flags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
				
				material.globalIlluminationFlags = flags;
			}
		}
		
		bool HasValidEmissiveKeyword (Material material)
		{
			// Material animation might be out of sync with the material keyword.
			// So if the emission support is disabled on the material, but the property blocks have a value that requires it, then we need to show a warning.
			// (note: (Renderer MaterialPropertyBlock applies its values to emissionColorForRendering))
			bool hasEmissionKeyword = material.IsKeywordEnabled ("_EMISSION");
			if (!hasEmissionKeyword && ShouldEmissionBeEnabled (emissionColorForRendering.colorValue))
				return false;
			else
				return true;
		}
		
		static void MaterialChanged(Material material, WorkflowMode workflowMode)
		{
			// Clamp EmissionScale to always positive
			if (material.GetFloat("_EmissionScaleUI") < 0.0f)
				material.SetFloat("_EmissionScaleUI", 0.0f);
			
			// Apply combined emission value
			Color emissionColorOut = EvalFinalEmissionColor (material);
			material.SetColor("_EmissionColor", emissionColorOut);

			if (workflowMode == WorkflowMode.Metallic)
			{
				material.SetFloat ("_UsingMetallicGlossMap", (material.GetTexture ("_MetallicGlossMap") == null) ? 0.0f : 1.0f);
			}
			else if (workflowMode == WorkflowMode.Specular)
			{
				material.SetFloat ("_UsingSpecGlossMap", (material.GetTexture ("_SpecGlossMap") == null) ? 0.0f : 1.0f);
			}

			SetMaterialKeywords(material, workflowMode);
		}
		
		static void SetKeyword(Material m, string keyword, bool state)
		{
			//Debug.Log(keyword + ": " + state);

			if (state)
				m.EnableKeyword (keyword);
			else
				m.DisableKeyword (keyword);
		}
	}
	
} // namespace UnityEditor
