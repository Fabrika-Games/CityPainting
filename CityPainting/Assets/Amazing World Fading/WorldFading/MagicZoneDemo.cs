using UnityEngine;

namespace AmazingWorldFading
{
	public class MagicZoneDemo : MonoBehaviour
	{
		public Shader[] m_Sdrs = new Shader[5];
		public GameObject m_MainCharacter;
		public bool m_ShowUI = true;
		[Header("Common")]
		[Range(0.1f, 64f)] public float m_VisDist = 6f;
		[Range(0f, 5f)] public float m_FadeWidth = 2f;
		public bool m_Inverse = false;
		[Header("Bright")]
		[Range(0f, 1f)] public float m_BrightIntensity = 0.3f;
		[Header("Desaturation")]
		[Range(0f, 1f)] public float m_Darkness = 0.8f;
		FadingControl[] m_Objs;
		int m_FadeFunc = 0;

		void Start()
		{
			m_Objs = GameObject.FindObjectsOfType<FadingControl>();
			for (int i = 0; i < m_Objs.Length; i++)
			{
				m_Objs[i].Initialize();
				m_Objs[i].ApplyShader(m_Sdrs[m_FadeFunc]);
				m_Objs[i].ApplyKeyword("AWD_SPHERICAL", true);
				m_Objs[i].ApplyKeyword("AWD_CUBIC", false);
				m_Objs[i].ApplyKeyword("AWD_ROUND_XZ", false);
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
				m_Objs[i].ApplyShader(m_Sdrs[m_FadeFunc]);
				m_Objs[i].ApplyKeyword("AWD_INVERSE", m_Inverse);
				m_Objs[i].UpdateSelfParameters();
				m_Objs[i].SetMaterialsVector("_Pos1", m_MainCharacter.transform.position);
				m_Objs[i].SetMaterialsFloat("_VisDist", m_VisDist);
				if (m_FadeFunc == 3)
					m_Objs[i].SetMaterialsFloat("_FadeWidth", m_FadeWidth / 4f);
				else
					m_Objs[i].SetMaterialsFloat("_FadeWidth", m_FadeWidth);
				m_Objs[i].SetMaterialsFloat("_FadeIntensity", m_BrightIntensity);
				m_Objs[i].SetMaterialsFloat("_Darkness", m_Darkness);
			}
		}
		void OnGUI()
		{
			if (!m_ShowUI)
				return;

			GUI.Box(new Rect (10, 10, 200, 25), "Magic Zone Demo");
			string[] names2 = { "Desaturation", "Bright", };
			m_FadeFunc = GUI.SelectionGrid(new Rect(10, 40, 100, 60), m_FadeFunc, names2, 1);
		}
	}
}
