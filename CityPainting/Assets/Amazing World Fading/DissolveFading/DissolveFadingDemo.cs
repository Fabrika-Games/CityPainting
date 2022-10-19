using UnityEngine;

namespace AmazingWorldFading
{
	public class DissolveFadingDemo : MonoBehaviour
	{
		public Transform m_MainCharacter;
		[Header("Sphere Dissolve")]
		[Range(1f, 16f)] public float m_VisDist = 9f;
		[Range(0f, 2f)] public float m_BorderRadius = 0.6f;
		public bool m_Invert = false;
		public Renderer[] m_ObjsSphereDissolve;
		[Header("Vertical Dissolve")]
		[Range(-0.1f, 1.6f)] public float m_Fill = 1f;
		public Renderer[] m_ObjsVerticalDissolve;

		void Update()
		{
			for (int i = 0; i < m_ObjsSphereDissolve.Length; i++)
			{
				m_ObjsSphereDissolve[i].material.SetVector("_Pos", m_MainCharacter.position);
				m_ObjsSphereDissolve[i].material.SetFloat("_Radius", m_VisDist);
				m_ObjsSphereDissolve[i].material.SetFloat("_Invert", m_Invert ? 1f : 0f);
				m_ObjsSphereDissolve[i].material.SetFloat("_BorderRadius", m_BorderRadius);
			}
			for (int i = 0; i < m_ObjsVerticalDissolve.Length; i++)
			{
				m_ObjsVerticalDissolve[i].material.SetFloat("_Fill", m_Fill);
			}
		}
	}
}
