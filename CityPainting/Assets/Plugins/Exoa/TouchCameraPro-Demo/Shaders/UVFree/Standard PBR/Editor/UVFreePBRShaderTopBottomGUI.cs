using System;
using UnityEngine;

namespace UnityEditor
{
	internal class UVFreePBRShaderTopBottomGUI : ShaderGUI
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
			public static GUIContent topMultiplierText = new GUIContent("Top Strength", "How much of the mesh should be considered the top. Set to 0 to disable.");

			public static GUIContent bottomMultiplierText = new GUIContent("Bottom Strength", "How much of the mesh should be considered the bottom. Set to 0 to disable.");
			public static GUIContent vertexColorStrengthText = new GUIContent("Vertex Color Strength", "0 for no vertex color (off), 1 for full vertex color tinting");

			public static GUIContent albedoLabel = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");

			public static GUIContent albedoText = new GUIContent("Main", "Albedo (RGB) and Transparency (A)");
			public static GUIContent albedoTextTop = new GUIContent("Top (+Y)", "Albedo (RGB) and Transparency (A)");
			public static GUIContent albedoTextBottom = new GUIContent("Bottom (-Y)", "Albedo (RGB) and Transparency (A)");

			public static GUIContent specularLabel = new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");

			public static GUIContent specularMapText = new GUIContent("Main", "Specular (RGB) and Smoothness (A)");
			public static GUIContent specularMapTextTop = new GUIContent("Top (+Y)", "Specular (RGB) and Smoothness (A)");
			public static GUIContent specularMapTextBottom = new GUIContent("Bottom (-Y)", "Specular (RGB) and Smoothness (A)");

			public static GUIContent metallicLabel = new GUIContent("Metallic", "Metallic (R) and Smoothness (A)");

			public static GUIContent metallicMapText = new GUIContent("Main", "Metallic (R) and Smoothness (A)");
			public static GUIContent metallicMapTextTop = new GUIContent("Top (+Y)", "Metallic (R) and Smoothness (A)");
			public static GUIContent metallicMapTextBottom = new GUIContent("Bottom (-Y)", "Metallic (R) and Smoothness (A)");

			public static GUIContent smoothnessText = new GUIContent("Smoothness", "");
			public static GUIContent smoothnessTextTop = new GUIContent("Smoothness", "");
			public static GUIContent smoothnessTextBottom = new GUIContent("Smoothness", "");

			public static GUIContent normalLabel = new GUIContent("Normal", "Normal Map");
			public static GUIContent normalMapText = new GUIContent("Main", "Normal Map");
			public static GUIContent normalMapTextTop = new GUIContent("Top (+Y)", "Normal Map");
			public static GUIContent normalMapTextBottom = new GUIContent("Bottom (-Y)", "Normal Map");

			public static GUIContent emissionLabel = new GUIContent("Emission", "Emission (RGB)");

			public static GUIContent emissionText = new GUIContent("Main", "Emission (RGB)");
			public static GUIContent emissionTextTop = new GUIContent("Top (+Y)", "Emission (RGB)");
			public static GUIContent emissionTextBottom = new GUIContent("Bottom (-Y)", "Emission (RGB)");

			public static string textureScaleOffset = "Main Texture Scale/Offset";
			public static string textureScaleOffsetTop = "Top (+Y) Texture Scale/Offset";
			public static string textureScaleOffsetBottom = "Bottom (-Y) Texture Scale/Offset";

			public static string whiteSpaceString = " ";

			public static GUIContent emissiveWarning = new GUIContent ("Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");
			public static GUIContent emissiveColorWarning = new GUIContent ("Ensure emissive color is non-black for emission to have effect.");
			public static readonly string[] spaceNames = Enum.GetNames (typeof(Space));
		}

		private const float MIN_VALUE = 0.0f;

		// FRONT
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
		MaterialProperty emissionScaleUI = null;
		MaterialProperty emissionColorUI = null;
		MaterialProperty emissionColorForRendering = null;
		MaterialProperty emissionMap = null;

		// TOP

		MaterialProperty albedoMapTop = null;
		MaterialProperty albedoColorTop = null;
		MaterialProperty specularMapTop = null;
		MaterialProperty specularColorTop = null;
		MaterialProperty metallicMapTop = null;
		MaterialProperty metallicTop = null;
		MaterialProperty smoothnessTop = null;
		MaterialProperty bumpScaleTop = null;
		MaterialProperty bumpMapTop = null;
		MaterialProperty emissionScaleUITop = null;
		MaterialProperty emissionColorUITop = null;
		MaterialProperty emissionMapTop = null;

		// BOTTOM
		
		MaterialProperty albedoMapBottom = null;
		MaterialProperty albedoColorBottom = null;
		MaterialProperty specularMapBottom = null;
		MaterialProperty specularColorBottom = null;
		MaterialProperty metallicMapBottom = null;
		MaterialProperty metallicBottom = null;
		MaterialProperty smoothnessBottom = null;
		MaterialProperty bumpScaleBottom = null;
		MaterialProperty bumpMapBottom = null;
		MaterialProperty emissionScaleUIBottom = null;
		MaterialProperty emissionColorUIBottom = null;
		MaterialProperty emissionMapBottom = null;

		MaterialProperty texturePower = null;

		MaterialProperty triplanarSpace = null;
		MaterialProperty topMultiplier = null;
		MaterialProperty bottomMultiplier = null;

		MaterialEditor m_MaterialEditor;
		WorkflowMode m_WorkflowMode = WorkflowMode.Specular;
		bool m_FirstTimeApply = true;
		
		public void FindProperties (MaterialProperty[] props)
		{
			triplanarSpace = FindProperty("_TriplanarSpace", props, false);
			texturePower = FindProperty("_TexPower", props, false);
			topMultiplier = FindProperty("_TopMultiplier", props, false);

			bottomMultiplier = FindProperty("_BottomMultiplier", props, false);

			vertexColorStrength = FindProperty("_VertexColorStrength", props, false);

			albedoMap = FindProperty ("_MainTex", props);
			albedoColor = FindProperty ("_Color", props);
			specularMap = FindProperty ("_SpecGlossMap", props, false);
			specularColor = FindProperty ("_SpecColor", props, false);
			metallicMap = FindProperty ("_MetallicGlossMap", props, false);
			metallic = FindProperty ("_Metallic", props, false);

			if (specularMap != null && specularColor != null)
			{
				m_WorkflowMode = WorkflowMode.Specular;
			}
			else if (metallicMap != null && metallic != null)
			{
				m_WorkflowMode = WorkflowMode.Metallic;
			}
			else
			{
				m_WorkflowMode = WorkflowMode.Dielectric;
			}

			smoothness = FindProperty ("_Glossiness", props);
			bumpScale = FindProperty ("_BumpScale", props);
			bumpMap = FindProperty ("_BumpMap", props);

			emissionMap = FindProperty("_EmissionMap", props);
			emissionScaleUI = FindProperty ("_EmissionScaleUI", props);
			emissionColorUI = FindProperty ("_EmissionColorUI", props);
			emissionColorForRendering = FindProperty ("_EmissionColor", props);

			// TOP

			albedoMapTop = FindProperty ("_TopMainTex", props, false);
			albedoColorTop = FindProperty ("_TopColor", props, false);
			specularMapTop = FindProperty ("_TopSpecGlossMap", props, false);
			specularColorTop = FindProperty ("_TopSpecColor", props, false);
			metallicMapTop = FindProperty ("_TopMetallicGlossMap", props, false);
			metallicTop = FindProperty ("_TopMetallic", props, false);
			smoothnessTop = FindProperty ("_TopGlossiness", props, false);
			bumpScaleTop = FindProperty ("_TopBumpScale", props, false);
			bumpMapTop = FindProperty ("_TopBumpMap", props, false);
			emissionMapTop = FindProperty("_TopEmissionMap", props);
			emissionScaleUITop = FindProperty ("_TopEmissionScaleUI", props);
			emissionColorUITop = FindProperty ("_TopEmissionColorUI", props);

			// BOTTOM
			
			albedoMapBottom = FindProperty ("_BottomMainTex", props, false);
			albedoColorBottom = FindProperty ("_BottomColor", props, false);

			specularMapBottom = FindProperty ("_BottomSpecGlossMap", props, false);
			specularColorBottom = FindProperty ("_BottomSpecColor", props, false);
			metallicMapBottom = FindProperty ("_BottomMetallicGlossMap", props, false);
			metallicBottom = FindProperty ("_BottomMetallic", props, false);
			smoothnessBottom = FindProperty ("_BottomGlossiness", props, false);
			bumpScaleBottom = FindProperty ("_BottomBumpScale", props, false);
			bumpMapBottom = FindProperty ("_BottomBumpMap", props, false);
			emissionMapBottom = FindProperty("_BottomEmissionMap", props, false);
			emissionScaleUIBottom = FindProperty ("_BottomEmissionScaleUI", props, false);
			emissionColorUIBottom = FindProperty ("_BottomEmissionColorUI", props, false);
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

				m_MaterialEditor.RangeProperty (topMultiplier, "Top Multiplier");
				m_MaterialEditor.RangeProperty (bottomMultiplier, "Bottom Multiplier");

				if (vertexColorStrength != null)
				{
					m_MaterialEditor.RangeProperty (vertexColorStrength, Styles.vertexColorStrengthText.text);
				}

				DoAlbedoArea(material);
				DoSpecularMetallicArea(material);

				// bump
				GUILayout.Label(Styles.normalLabel, EditorStyles.boldLabel);

				m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
				if (topMultiplier.floatValue > MIN_VALUE)
				{
					m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapTextTop, bumpMapTop, bumpMapTop.textureValue != null ? bumpScaleTop : null);
				}
				if (bottomMultiplier.floatValue > MIN_VALUE)
				{
					m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapTextBottom, bumpMapBottom, bumpMapBottom.textureValue != null ? bumpScaleBottom : null);
				}

				DoEmissionArea(material);

				// MAIN
				EditorGUILayout.Space();
				GUILayout.Label(Styles.textureScaleOffset, EditorStyles.boldLabel);
				m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);

				// TOP
				if (topMultiplier.floatValue > MIN_VALUE)
				{
					EditorGUILayout.Space();
					GUILayout.Label(Styles.textureScaleOffsetTop, EditorStyles.boldLabel);
					m_MaterialEditor.TextureScaleOffsetProperty(albedoMapTop);
				}

				// BOTTOM
				if (bottomMultiplier.floatValue > MIN_VALUE)
				{
					EditorGUILayout.Space();
					GUILayout.Label(Styles.textureScaleOffsetBottom, EditorStyles.boldLabel);
					m_MaterialEditor.TextureScaleOffsetProperty(albedoMapBottom);
				}
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
			GUILayout.Label(Styles.albedoLabel, EditorStyles.boldLabel);

			m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);

			if (material.GetFloat ("_TopMultiplier") > MIN_VALUE)
			{
				m_MaterialEditor.TexturePropertySingleLine(Styles.albedoTextTop, albedoMapTop, albedoColorTop);
			}

			if (material.GetFloat ("_BottomMultiplier") > MIN_VALUE)
			{
				m_MaterialEditor.TexturePropertySingleLine(Styles.albedoTextBottom, albedoMapBottom, albedoColorBottom);
			}

		}

		void DoEmissionArea(Material material)
		{
			GUILayout.Label(Styles.emissionLabel, EditorStyles.boldLabel);

			bool showEmissionColorAndGIControls = emissionScaleUI.floatValue > 0f;
			bool showEmissionColorAndGIControlsTop = emissionScaleUITop.floatValue > 0f;
			bool showEmissionColorAndGIControlsBottom = emissionScaleUIBottom.floatValue > 0f;

			// Do controls
			m_MaterialEditor.TexturePropertySingleLine(Styles.emissionText, emissionMap, showEmissionColorAndGIControls ? emissionColorUI : null, emissionScaleUI);

			if (material.GetFloat ("_TopMultiplier") > MIN_VALUE)
			{
				m_MaterialEditor.TexturePropertySingleLine(Styles.emissionTextTop, emissionMapTop, showEmissionColorAndGIControlsTop ? emissionColorUITop : null, emissionScaleUITop);
			}

			if (material.GetFloat("_BottomMultiplier") > MIN_VALUE)
			{
				m_MaterialEditor.TexturePropertySingleLine(Styles.emissionTextBottom, emissionMapBottom, showEmissionColorAndGIControlsBottom ? emissionColorUIBottom : null, emissionScaleUIBottom);
			}

			// Dynamic Lightmapping mode
			if (showEmissionColorAndGIControls)
			{
				bool shouldEmissionBeEnabled = ShouldEmissionBeEnabled(EvalFinalEmissionColor(material));
				EditorGUI.BeginDisabledGroup(!shouldEmissionBeEnabled);
				
				m_MaterialEditor.LightmapEmissionProperty (MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
				
				EditorGUI.EndDisabledGroup();
			}

		}

		void DoSpecularMetallicArea(Material material)
		{
			if (m_WorkflowMode == WorkflowMode.Specular)
			{
				GUILayout.Label(Styles.specularLabel, EditorStyles.boldLabel);

				// MAIN
				if (specularMap.textureValue == null)
				{
					m_MaterialEditor.TexturePropertyTwoLines(Styles.specularMapText, specularMap, specularColor, Styles.smoothnessText, smoothness);
				}
				else
				{
					m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, specularMap);
				}

				// TOP
				if (topMultiplier.floatValue > MIN_VALUE)
				{

					if (specularMapTop.textureValue == null)
					{
						m_MaterialEditor.TexturePropertyTwoLines(Styles.specularMapTextTop, specularMapTop, specularColorTop, Styles.smoothnessTextTop, smoothnessTop);
					}
					else
					{
						m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapTextTop, specularMapTop);
					}
				}

				// BOTTOM
				if (bottomMultiplier.floatValue > MIN_VALUE)
				{

					if (specularMapBottom.textureValue == null)
					{
						m_MaterialEditor.TexturePropertyTwoLines(Styles.specularMapTextBottom, specularMapBottom, specularColorBottom, Styles.smoothnessTextBottom, smoothnessBottom);
					}
					else
					{
						m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapTextBottom, specularMapBottom);
					}
				}
				
			}
			else if (m_WorkflowMode == WorkflowMode.Metallic)
			{
				GUILayout.Label(Styles.metallicLabel, EditorStyles.boldLabel);

				// MAIN
				if (metallicMap.textureValue == null)
				{
					m_MaterialEditor.TexturePropertyTwoLines(Styles.metallicMapText, metallicMap, metallic, Styles.smoothnessText, smoothness);

				}
				else
				{
					m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicMap);
				}

				// TOP
				if (topMultiplier.floatValue > MIN_VALUE)
				{

					if (metallicMapTop.textureValue == null)
					{
						m_MaterialEditor.TexturePropertyTwoLines(Styles.metallicMapTextTop, metallicMapTop, metallicTop, Styles.smoothnessTextTop, smoothnessTop);
					}
					else
					{
						m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapTextTop, metallicMapTop);
					}
				}

				// BOTTOM
				if (bottomMultiplier.floatValue > MIN_VALUE)
				{
					if (metallicMapBottom.textureValue == null)
					{
						m_MaterialEditor.TexturePropertyTwoLines(Styles.metallicMapTextBottom, metallicMapBottom, metallicBottom, Styles.smoothnessTextBottom, smoothnessBottom);
					}
					else
					{
						m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapTextBottom, metallicMapBottom);
					}
				}

			}
		}
		
		// Calculate final HDR _EmissionColor (gamma space) from _EmissionColorUI (LDR, gamma) & _EmissionScaleUI (gamma)
		static Color EvalFinalEmissionColor(Material material)
		{
			return material.GetColor("_EmissionColorUI") * material.GetFloat("_EmissionScaleUI");
		}


		static Color EvalFinalEmissionColorTop(Material material)
		{
			return material.GetColor("_TopEmissionColorUI") * material.GetFloat("_TopEmissionScaleUI");
		}
		static Color EvalFinalEmissionColorBottom(Material material)
		{
			return material.GetColor("_BottomEmissionColorUI") * material.GetFloat("_BottomEmissionScaleUI");
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
			{
				SetKeyword (material, "_SPECGLOSSMAP", 
				            material.GetTexture ("_SpecGlossMap") ||
				            material.GetTexture ("_TopSpecGlossMap") ||
				            material.GetTexture ("_BottomSpecGlossMap")
				            );
			}
			else if (workflowMode == WorkflowMode.Metallic)
			{
				SetKeyword (material, "_METALLICGLOSSMAP", 
				            material.GetTexture ("_MetallicGlossMap") ||
				            material.GetTexture ("_TopMetallicGlossMap") ||
				            material.GetTexture ("_BottomMetallicGlossMap")
				            );
			}

			bool shouldEmissionBeEnabled = 
				ShouldEmissionBeEnabled (material.GetColor("_EmissionColor"))
			 || (material.HasProperty ("_TopMultiplier") && material.GetFloat ("_TopMultiplier") > MIN_VALUE && ShouldEmissionBeEnabled (material.GetColor("_TopEmissionColor")))
			 || (material.HasProperty ("_BottomMultiplier") && material.GetFloat ("_BottomMultiplier") > MIN_VALUE && ShouldEmissionBeEnabled (material.GetColor("_BottomEmissionColor")));

			SetKeyword (material, "_EMISSION", shouldEmissionBeEnabled);

			// Triplanar texture space
			SetKeyword(material, "_UVFREE_LOCAL", material.GetFloat ("_TriplanarSpace") == (float) Space.Self);

			// whether the bottom is enabled
			SetKeyword(material, "_UVFREE_BOTTOM", material.GetFloat ("_BottomMultiplier") > (0.1f / 255.0f));

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
			// clamp texture power
			float tTexPower = material.GetFloat("_TexPower");
			if (tTexPower < 0.0f || tTexPower > 20.0f)
			{
				material.SetFloat ("_TexPower", Mathf.Clamp (tTexPower, 0.0f, 20.0f));
			}

			// clamp top multiplier
			float tTopMultiplier = material.GetFloat ("_TopMultiplier");
			if (tTopMultiplier < 0.0f || tTopMultiplier > 8.0f)
			{
				material.SetFloat ("_TopMultiplier", Mathf.Clamp (tTopMultiplier, 0.0f, 8.0f));
			}

			// clamp bottom multiplier
			float tBottomMultiplier = material.GetFloat ("_BottomMultiplier");
			if (tBottomMultiplier < 0.0f || tBottomMultiplier > 8.0f)
			{
				material.SetFloat ("_BottomMultiplier", Mathf.Clamp (tBottomMultiplier, 0.0f, 8.0f));
			}

			// Clamp EmissionScale to always positive
			if (material.GetFloat("_EmissionScaleUI") < 0.0f)
				material.SetFloat("_EmissionScaleUI", 0.0f);
			
			// Apply combined emission value
			Color emissionColorOut = EvalFinalEmissionColor (material);
			material.SetColor("_EmissionColor", emissionColorOut);


			// TOP
			
			if (material.GetFloat("_TopEmissionScaleUI") < 0.0f)
				material.SetFloat("_TopEmissionScaleUI", 0.0f);
			
			Color emissionColorOutTop = EvalFinalEmissionColorTop (material);
			material.SetColor("_TopEmissionColor", emissionColorOutTop);

			// BOTTOM

			if (material.GetFloat("_BottomEmissionScaleUI") < 0.0f)
				material.SetFloat("_BottomEmissionScaleUI", 0.0f);
			
			Color emissionColorOutBottom = EvalFinalEmissionColorBottom (material);
			material.SetColor("_BottomEmissionColor", emissionColorOutBottom);


			if (workflowMode == WorkflowMode.Metallic)
			{
				material.SetFloat ("_UsingMetallicGlossMap", (material.GetTexture ("_MetallicGlossMap") == null) ? 0.0f : 1.0f);
				material.SetFloat ("_TopUsingMetallicGlossMap", (material.GetTexture ("_TopMetallicGlossMap") == null) ? 0.0f : 1.0f);
				material.SetFloat ("_BottomUsingMetallicGlossMap", (material.GetTexture ("_BottomMetallicGlossMap") == null) ? 0.0f : 1.0f);
			}
			else if (workflowMode == WorkflowMode.Specular)
			{
				material.SetFloat ("_UsingSpecGlossMap", (material.GetTexture ("_SpecGlossMap") == null) ? 0.0f : 1.0f);
				material.SetFloat ("_TopUsingSpecGlossMap", (material.GetTexture ("_TopSpecGlossMap") == null) ? 0.0f : 1.0f);
				material.SetFloat ("_BottomUsingSpecGlossMap", (material.GetTexture ("_BottomSpecGlossMap") == null) ? 0.0f : 1.0f);
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
