using UnityEngine;

namespace AmazingWorldFading
{
	public class WorldFadingDemo : MonoBehaviour
	{
		[Header("Context")]
		public Shader[] m_Sdrs = new Shader[5];
		public bool m_ShowUI = true;
		[Header("Fading")]
		public Transform m_MainCharacter;
		public Transform[] m_Others;
		[Range(0.1f, 64f)] public float m_VisDist = 6f;
		[Range(0f, 5f)] public float m_FadeWidth = 2f;
		public bool m_Inverse = false;
		[Header("Halo")]
		public bool m_HaloEdgeNoise = false;
		[Header("Bright")]
		[Range(0f, 1f)] public float m_BrightIntensity = 0.3f;
		[Header("Desaturation")]
		[Range(0f, 1f)] public float m_Darkness = 0.8f;
		[Header("Stripe")]
		[Range(0f, 0.8f)] public float m_StripeWidth = 0.1f;
		[Range(0.1f, 20f)] public float m_StripeDensity = 5f;
		FadingControl[] m_Objs;
		int m_FadeShape = 0;
		int m_FadeFunc = 0;

		void Start()
		{
			m_Objs = GameObject.FindObjectsOfType<FadingControl>();
			for (int i = 0; i < m_Objs.Length; i++)
			{
				m_Objs[i].Initialize();
				m_Objs[i].ApplyShader(m_Sdrs[m_FadeFunc]);
				m_Objs[i].ApplyKeyword("AWD_POS1", true);
				m_Objs[i].ApplyKeyword("AWD_POS2", false);
				m_Objs[i].ApplyKeyword("AWD_POS3", false);
				m_Objs[i].ApplyKeyword("AWD_POS4", false);
			}
		}
		void Update()
		{
			for (int i = 0; i < m_Objs.Length; i++)
			{
				FadingControl fc = m_Objs[i];
				fc.ApplyShader(m_Sdrs[m_FadeFunc]);
				if (m_FadeShape == 0)
				{
					fc.ApplyKeyword("AWD_SPHERICAL", true);
					fc.ApplyKeyword("AWD_CUBIC", false);
					fc.ApplyKeyword("AWD_ROUND_XZ", false);
				}
				else if (m_FadeShape == 1)
				{
					fc.ApplyKeyword("AWD_CUBIC", true);
					fc.ApplyKeyword("AWD_SPHERICAL", false);
					fc.ApplyKeyword("AWD_ROUND_XZ", false);
				}
				else if (m_FadeShape == 2)
				{
					fc.ApplyKeyword("AWD_ROUND_XZ", true);
					fc.ApplyKeyword("AWD_CUBIC", false);
					fc.ApplyKeyword("AWD_SPHERICAL", false);
				}
				fc.ApplyKeyword("AWD_INVERSE", m_Inverse);
				if (m_FadeFunc == 0)
					fc.ApplyKeyword("AWD_NOISEHALO", m_HaloEdgeNoise);

				fc.UpdateSelfParameters();

				if (m_MainCharacter)
					fc.SetMaterialsVector("_Pos1", m_MainCharacter.position);

				if (m_Others.Length > 0)
				{
					if (m_Others.Length == 1)
					{
						fc.ApplyKeyword("AWD_POS1", false);
						fc.ApplyKeyword("AWD_POS2", true);
						fc.ApplyKeyword("AWD_POS3", false);
						fc.ApplyKeyword("AWD_POS4", false);
						fc.SetMaterialsVector("_Pos2", m_Others[0].position);
					}
					if (m_Others.Length == 2)
					{
						fc.ApplyKeyword("AWD_POS1", false);
						fc.ApplyKeyword("AWD_POS2", false);
						fc.ApplyKeyword("AWD_POS3", true);
						fc.ApplyKeyword("AWD_POS4", false);
						fc.SetMaterialsVector("_Pos2", m_Others[0].position);
						fc.SetMaterialsVector("_Pos3", m_Others[1].position);
					}
					if (m_Others.Length == 3)
					{
						fc.ApplyKeyword("AWD_POS1", false);
						fc.ApplyKeyword("AWD_POS2", false);
						fc.ApplyKeyword("AWD_POS3", false);
						fc.ApplyKeyword("AWD_POS4", true);
						fc.SetMaterialsVector("_Pos2", m_Others[0].position);
						fc.SetMaterialsVector("_Pos3", m_Others[1].position);
						fc.SetMaterialsVector("_Pos4", m_Others[2].position);
					}
				}

				fc.SetMaterialsFloat("_VisDist", m_VisDist);
				if (m_FadeFunc == 3)
					fc.SetMaterialsFloat("_FadeWidth", m_FadeWidth / 4f);
				else
					fc.SetMaterialsFloat("_FadeWidth", m_FadeWidth);
				fc.SetMaterialsFloat("_FadeIntensity", m_BrightIntensity);
				fc.SetMaterialsFloat("_Darkness", m_Darkness);
				fc.SetMaterialsFloat("_StripeWidth", m_StripeWidth);
				fc.SetMaterialsFloat("_StripeDensity", m_StripeDensity);
			}
		}
		void OnGUI()
		{
			if (!m_ShowUI)
				return;

			GUI.Box(new Rect(10, 10, 210, 25), "Amazing World Fading Demo");

			//if (m_Others.Length > 0)
			//{
			//	string[] names = { "Spherical", "Cubic" };
			//	m_FadeShape = GUI.SelectionGrid (new Rect(10, 40, 100, 50), m_FadeShape, names, 1);
			//	string[] names2 = { "Halo", "Bright", "Desaturation", "2 Textures", "Stripe" };
			//	m_FadeFunc = GUI.SelectionGrid(new Rect(10, 100, 100, 150), m_FadeFunc, names2, 1);
			//}
			//else
			{
				string[] names = { "Spherical", "Cubic", "Cylinder" };
				m_FadeShape = GUI.SelectionGrid (new Rect(10, 40, 100, 80), m_FadeShape, names, 1);
				string[] names2 = { "Halo", "Bright", "Desaturation", "2 Textures", "Stripe" };
				m_FadeFunc = GUI.SelectionGrid(new Rect(10, 140, 100, 150), m_FadeFunc, names2, 1);
			}
		}
	}
}