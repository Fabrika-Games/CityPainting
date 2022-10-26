using System;
using UnityEngine;

namespace UnityEditor
{
	internal class UVFreeLegacyTopBottomGUI : ShaderGUI
	{
		private enum WorkflowMode
		{
			Lambert,
			BlinnPhong
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

			public static GUIContent baseColorLabel = new GUIContent("Diffuse", "Base Color (RGB) Gloss(A)");
			public static GUIContent baseColorText = new GUIContent("Diffuse (RGB) Gloss (A)", "Base Color (RGB) Gloss (A)");
			public static GUIContent baseColorTextMain = new GUIContent("Main", "Base Color (RGB) Gloss (A)");
			public static GUIContent baseColorTextTop = new GUIContent("Top (+Y)", "Base Color (RGB) Gloss (A)");
			public static GUIContent baseColorTextBottom = new GUIContent("Bottom (-Y)", "Base Color (RGB) Gloss (A)");

			public static GUIContent normalLabel = new GUIContent("Normal", "Normal Map");
			public static GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map");
			public static GUIContent normalMapMain = new GUIContent("Main", "Normal Map");
			public static GUIContent normalMapTextTop = new GUIContent("Top (+Y)", "Normal Map");
			public static GUIContent normalMapTextBottom = new GUIContent("Bottom (-Y)", "Normal Map");

			public static GUIContent specLabel = new GUIContent("Specular", "Specular");
			public static GUIContent specColorText = new GUIContent("Specular Color", "Specular Color");
			public static GUIContent shininessText = new GUIContent("Shininess", "Shininess");

			public static GUIContent shininessTextMain = new GUIContent("Main", "Shininess");
			public static GUIContent shininessTextTop = new GUIContent("Top (+Y)", "Shininess");
			public static GUIContent shininessTextBottom = new GUIContent("Bottom (-Y)", "Shininess");

			public static GUIContent rimLabel = new GUIContent("Rim Lighting", "Rim Lighting");

			public static GUIContent rimText = new GUIContent("Rim", "");
			public static GUIContent rimColor = new GUIContent("Color", "");
			public static GUIContent rimPower = new GUIContent("Power", "");
			public static GUIContent rimMultiplier = new GUIContent("Strength", "");

			public static GUIContent emissionLabel = new GUIContent("Emission", "Emission (RGB)");
			
			public static GUIContent emissionText = new GUIContent("Main", "Emission (RGB)");
			public static GUIContent emissionTextTop = new GUIContent("Top (+Y)", "Emission (RGB)");
			public static GUIContent emissionTextBottom = new GUIContent("Bottom (-Y)", "Emission (RGB)");


			public static string textureScaleOffset = "Main Texture Scale/Offset";
			public static string textureScaleOffsetTop = "Top (+Y) Texture Scale/Offset";
			public static string textureScaleOffsetBottom = "Bottom (-Y) Texture Scale/Offset";

			public static string whiteSpaceString = " ";

			public static readonly string[] spaceNames = Enum.GetNames (typeof(Space));
		}

		private const float MIN_VALUE = 0.0f;

		MaterialProperty uvFreeLocal = null;

		// FRONT

		MaterialProperty texPower = null;
		MaterialProperty topMultiplier = null;
		
		MaterialProperty bottomMultiplier = null;
		MaterialProperty vertexColorStrength = null;

		// MAIN
		
		MaterialProperty color = null;
		MaterialProperty mainTex = null;

		MaterialProperty bumpMap = null;
		MaterialProperty bumpScale = null;

		MaterialProperty specColor = null;
		MaterialProperty shininess = null;

		MaterialProperty rimColor = null;
		MaterialProperty rimPower = null;
		MaterialProperty rimMultiplier = null;
		
		// TOP
		MaterialProperty topColor = null;
		MaterialProperty topMainTex = null;

		MaterialProperty topBumpMap = null;
		MaterialProperty topBumpScale = null;
		
		MaterialProperty topShininess = null;
		
		// BOTTOM
		MaterialProperty bottomColor = null;
		MaterialProperty bottomMainTex = null;

		MaterialProperty bottomBumpMap = null;
		MaterialProperty bottomBumpScale = null;
		
		MaterialProperty bottomShininess = null;

		MaterialProperty emissionMap = null;
		MaterialProperty emissionColor = null;
		MaterialProperty emissionMultiplier = null;

		MaterialProperty topEmissionMap = null;
		MaterialProperty topEmissionColor = null;
		MaterialProperty topEmissionMultiplier = null;

		MaterialProperty bottomEmissionMap = null;
		MaterialProperty bottomEmissionColor = null;
		MaterialProperty bottomEmissionMultiplier = null;

		MaterialEditor m_MaterialEditor;

		WorkflowMode m_WorkflowMode = WorkflowMode.BlinnPhong;

		bool m_FirstTimeApply = true;
		
		public void FindProperties (MaterialProperty[] props)
		{
			texPower = FindProperty("_TexPower", props, false);
			topMultiplier = FindProperty("_TopMultiplier", props, false);
			bottomMultiplier = FindProperty("_BottomMultiplier", props, false);

			uvFreeLocal = FindProperty("_UVFreeLocal", props, false);

			vertexColorStrength = FindProperty("_VertexColorStrength", props, false);

			// MAIN

			color = FindProperty("_Color", props, false);
			mainTex = FindProperty("_MainTex", props, false);

			bumpMap = FindProperty("_BumpMap", props, false);
			bumpScale = FindProperty("_BumpScale", props, false);
			
			specColor = FindProperty("_SpecColor", props, false);
			shininess = FindProperty("_Shininess", props, false);

			m_WorkflowMode = shininess == null ? WorkflowMode.Lambert : WorkflowMode.BlinnPhong;

			rimColor = FindProperty("_RimColor", props, false);
			rimPower = FindProperty("_RimPower", props, false);
			rimMultiplier = FindProperty("_RimMultiplier", props, false);

			emissionColor = FindProperty("_EmissionColor", props, false);
			emissionMap = FindProperty("_EmissionMap", props, false);
			emissionMultiplier = FindProperty("_EmissionMultiplier", props, false);

			// TOP

			topColor = FindProperty("_TopColor", props, false);
			topMainTex = FindProperty("_TopMainTex", props, false);

			topBumpMap = FindProperty("_TopBumpMap", props, false);
			topBumpScale = FindProperty("_TopBumpScale", props, false);
			
			topShininess = FindProperty("_TopShininess", props, false);

			topEmissionColor = FindProperty("_TopEmissionColor", props, false);
			topEmissionMap = FindProperty("_TopEmissionMap", props, false);
			topEmissionMultiplier = FindProperty("_TopEmissionMultiplier", props, false);

			// BOTTOM

			bottomColor = FindProperty("_BottomColor", props, false);
			bottomMainTex = FindProperty("_BottomMainTex", props, false);

			bottomBumpMap = FindProperty("_BottomBumpMap", props, false);
			bottomBumpScale = FindProperty("_BottomBumpScale", props, false);
			
			bottomShininess = FindProperty("_BottomShininess", props, false);

			bottomEmissionColor = FindProperty("_BottomEmissionColor", props, false);
			bottomEmissionMap = FindProperty("_BottomEmissionMap", props, false);
			bottomEmissionMultiplier = FindProperty("_BottomEmissionMultiplier", props, false);

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
				m_MaterialEditor.RangeProperty (texPower, Styles.texturePowerText.text);

				if (bottomMultiplier != null)
				{
					m_MaterialEditor.RangeProperty (topMultiplier, Styles.topMultiplierText.text);
					m_MaterialEditor.RangeProperty (bottomMultiplier, Styles.bottomMultiplierText.text);
				}

				if (vertexColorStrength != null)
				{
					m_MaterialEditor.RangeProperty (vertexColorStrength, Styles.vertexColorStrengthText.text);
				}

				DoAlbedoArea(material);
				EditorGUILayout.Space();

				DoSpecularArea(material);
				EditorGUILayout.Space();

				DoBumpArea(material);
				EditorGUILayout.Space();

				DoEmissionArea(material);
				EditorGUILayout.Space ();

				DoTextureScaleArea (material);
				EditorGUILayout.Space();

				DoRimArea(material);

			}
			if (EditorGUI.EndChangeCheck())
			{
				MaterialChanged(material, m_WorkflowMode);
			}
		}

		void DoEmissionArea(Material material)
		{

			GUILayout.Label(Styles.emissionLabel, EditorStyles.boldLabel);
			
			bool showEmissionColorAndGIControls = emissionMultiplier.floatValue > 0f;
			bool showEmissionColorAndGIControlsTop = topEmissionMultiplier != null && topEmissionMultiplier.floatValue > 0f;
			bool showEmissionColorAndGIControlsBottom = bottomEmissionMultiplier != null && bottomEmissionMultiplier.floatValue > 0f;
			
			// Do controls
			m_MaterialEditor.TexturePropertySingleLine(Styles.emissionText, emissionMap, showEmissionColorAndGIControls ? emissionColor : null, emissionMultiplier);
			
			if (topMultiplier != null && topMultiplier.floatValue > MIN_VALUE)
			{
				m_MaterialEditor.TexturePropertySingleLine(Styles.emissionTextTop, topEmissionMap, showEmissionColorAndGIControlsTop ? topEmissionColor : null, topEmissionMultiplier);
			}
			
			if (bottomMultiplier != null && bottomMultiplier.floatValue > MIN_VALUE)
			{
				m_MaterialEditor.TexturePropertySingleLine(Styles.emissionTextBottom, bottomEmissionMap, showEmissionColorAndGIControlsBottom ? bottomEmissionColor : null, bottomEmissionMultiplier);
			}
			
			// Dynamic Lightmapping mode
			if (showEmissionColorAndGIControls)
			{
				bool shouldEmissionBeEnabled = showEmissionColorAndGIControls
					|| showEmissionColorAndGIControlsTop
					|| showEmissionColorAndGIControlsBottom;

				EditorGUI.BeginDisabledGroup(!shouldEmissionBeEnabled);
				
				m_MaterialEditor.LightmapEmissionProperty (MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
				
				EditorGUI.EndDisabledGroup();
			}

		}

		void DoRimArea(Material material)
		{
			GUILayout.Label (Styles.rimLabel, EditorStyles.boldLabel);

			m_MaterialEditor.RangeProperty (rimMultiplier, Styles.rimMultiplier.text);
			if (rimMultiplier.floatValue > MIN_VALUE)
			{
				m_MaterialEditor.ColorProperty (rimColor, Styles.rimColor.text);
				m_MaterialEditor.RangeProperty (rimPower, Styles.rimPower.text);
			}
		}

		void DoTextureScaleArea(Material material)
		{
			GUILayout.Label (Styles.textureScaleOffset, EditorStyles.boldLabel);
			m_MaterialEditor.TextureScaleOffsetProperty (mainTex);
			if (bottomMultiplier != null) {
				// TOP
				if (topMultiplier.floatValue > MIN_VALUE) {
					EditorGUILayout.Space ();
					GUILayout.Label (Styles.textureScaleOffsetTop, EditorStyles.boldLabel);
					m_MaterialEditor.TextureScaleOffsetProperty (topMainTex);
				}
				// BOTTOM
				if (bottomMultiplier.floatValue > MIN_VALUE) {
					EditorGUILayout.Space ();
					GUILayout.Label (Styles.textureScaleOffsetBottom, EditorStyles.boldLabel);
					m_MaterialEditor.TextureScaleOffsetProperty (bottomMainTex);
				}
			}
		}

		void DoBumpArea(Material material)
		{
			// bump
			GUILayout.Label (Styles.normalLabel, EditorStyles.boldLabel);
			if ((bottomMultiplier != null)) {
				m_MaterialEditor.TexturePropertySingleLine (Styles.normalMapMain, bumpMap, bumpMap.textureValue != null ? bumpScale : null);

				if (topMultiplier.floatValue > MIN_VALUE) {
					m_MaterialEditor.TexturePropertySingleLine (Styles.normalMapTextTop, topBumpMap, topBumpMap.textureValue != null ? topBumpScale : null);
				}
				if (bottomMultiplier.floatValue > MIN_VALUE) {
					m_MaterialEditor.TexturePropertySingleLine (Styles.normalMapTextBottom, bottomBumpMap, bottomBumpMap.textureValue != null ? bottomBumpScale : null);
				}
			}
			else
			{
				m_MaterialEditor.TexturePropertySingleLine (Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
			}
		}

		public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
		{
			base.AssignNewShaderToMaterial(material, oldShader, newShader);
		}


		void TriplanarSpacePopup()
		{
			EditorGUI.showMixedValue = uvFreeLocal.hasMixedValue;

			int space = (uvFreeLocal.floatValue == 0.0f) ? ((int) Space.World) : ((int) Space.Self);
			
			EditorGUI.BeginChangeCheck();
			space = EditorGUILayout.Popup(Styles.triplanarSpace.text, space, Styles.spaceNames);

			if (EditorGUI.EndChangeCheck())
			{
				m_MaterialEditor.RegisterPropertyChangeUndo("Triplanar Space");
				uvFreeLocal.floatValue = (space == ((int) Space.World)) ? 0.0f : 1.0f;
			}
			
			EditorGUI.showMixedValue = false;

		}

		void DoAlbedoArea(Material material)
		{
			GUILayout.Label(Styles.baseColorLabel, EditorStyles.boldLabel);
			m_MaterialEditor.TexturePropertySingleLine(Styles.baseColorTextMain, mainTex, color);

			// top bottom
			if (bottomMultiplier != null)
			{

				if (topMultiplier.floatValue > MIN_VALUE)
				{
					m_MaterialEditor.TexturePropertySingleLine(Styles.baseColorTextTop, topMainTex, topColor);
				}
				if (bottomMultiplier.floatValue > MIN_VALUE)
				{
					m_MaterialEditor.TexturePropertySingleLine(Styles.baseColorTextBottom, bottomMainTex, bottomColor);
				}
			}


		}

		void DoSpecularArea(Material material)
		{
			if (m_WorkflowMode == WorkflowMode.BlinnPhong)
			{
				GUILayout.Label(Styles.specLabel, EditorStyles.boldLabel);

				m_MaterialEditor.ColorProperty (specColor, Styles.specColorText.text);

				// top and bottom
				if (bottomMultiplier != null)
				{
					EditorGUILayout.LabelField (Styles.shininessText.text);
					EditorGUI.indentLevel++;
					m_MaterialEditor.RangeProperty (shininess, Styles.shininessTextMain.text);

					if (topMultiplier.floatValue > MIN_VALUE)
					{
						m_MaterialEditor.RangeProperty (topShininess, Styles.shininessTextTop.text);
					}

					if (bottomMultiplier.floatValue > MIN_VALUE)
					{
						m_MaterialEditor.RangeProperty (bottomShininess, Styles.shininessTextBottom.text);
					}
					EditorGUI.indentLevel--;
				}
				else // single tex
				{
					m_MaterialEditor.RangeProperty (shininess, Styles.shininessText.text);
				}

			}
		}

		static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
		{
			// Triplanar texture space
			SetKeyword(material, "_UVFREE_LOCAL", material.GetFloat ("_UVFreeLocal") == (float) Space.Self);

			if (material.HasProperty("_BottomMultiplier"))
			{
				// whether the bottom is enabled
				bool bottomEnabled = material.GetFloat ("_BottomMultiplier") > MIN_VALUE;
				SetKeyword(material, "_UVFREE_BOTTOM", bottomEnabled);

			}

			// vertex color
			SetKeyword(material, "_UVFREE_VERTEX_COLOR", material.GetFloat ("_VertexColorStrength") > MIN_VALUE);

			// Rim
			SetKeyword(material, "_UVFREE_RIM", material.GetFloat ("_RimMultiplier") > MIN_VALUE);

			// Bump

			bool includeBump = Mathf.Abs (material.GetFloat ("_BumpScale")) > MIN_VALUE;

			if (material.HasProperty ("_BottomMultiplier"))
			{
				if (material.GetFloat ("_TopMultiplier") > MIN_VALUE)
				{
					includeBump = includeBump || (material.HasProperty ("_TopBumpScale") && Mathf.Abs (material.GetFloat ("_TopBumpScale")) > MIN_VALUE);
				}
				if (material.GetFloat ("_BottomMultiplier") > MIN_VALUE)
				{
					includeBump = includeBump || (material.HasProperty ("_BottomBumpScale") && Mathf.Abs (material.GetFloat ("_BottomBumpScale")) > MIN_VALUE);
				}

			}
			SetKeyword(material, "_UVFREE_BUMPED", includeBump);

			// emission
			if (material.HasProperty ("_EmissionMultiplier"))
			{
				bool includeEmission = material.HasProperty ("_EmissionMultiplier") && material.GetFloat("_EmissionMultiplier") > MIN_VALUE;

				if (material.HasProperty("_TopMultiplier") 
				    && material.GetFloat("_TopMultiplier") > MIN_VALUE)
				{
					includeEmission = includeEmission || (material.HasProperty ("_TopEmissionMultiplier") && material.GetFloat("_TopEmissionMultiplier") > MIN_VALUE);
				}
				if (material.HasProperty("_BottomMultiplier") 
				    && material.GetFloat("_BottomMultiplier") > MIN_VALUE)
				{
					includeEmission = includeEmission || (material.HasProperty ("_BottomEmissionMultiplier") && material.GetFloat("_BottomEmissionMultiplier") > MIN_VALUE);
				}
				SetKeyword(material, "_EMISSION", includeEmission);

			}
		}
		
		static void MaterialChanged(Material material, WorkflowMode workflowMode)
		{
			// clamp texture power
			float tTexPower = material.GetFloat("_TexPower");
			if (tTexPower < 0.0f || tTexPower > 20.0f)
			{
				material.SetFloat ("_TexPower", Mathf.Clamp (tTexPower, 0.0f, 20.0f));
			}

			if (material.HasProperty ("_BottomMultiplier"))
			{
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
			}

			SetMaterialKeywords(material, workflowMode);
		}
		
		static void SetKeyword(Material m, string keyword, bool state)
		{
			if (state)
				m.EnableKeyword (keyword);
			else
				m.DisableKeyword (keyword);

			//Debug.Log ("Keyword: " + keyword + ": " + state);
		}
	}
	
} // namespace UnityEditor
