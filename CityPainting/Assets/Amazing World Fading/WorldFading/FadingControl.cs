using UnityEngine;

namespace AmazingWorldFading
{
	public class FadingControl : MonoBehaviour
	{
		public float m_HaloBloom = 1f;
		public Texture2D m_SecondaryTex;
		public Vector2 m_SecondaryTexScale = Vector2.one;
		public Texture2D m_StripeTex;
		public Color m_StripeColor = Color.green;
		public float m_StripeBloom = 3f;
		public float m_Blend2TexBloom = 3f;
		Renderer m_Rd;

		public void Initialize()
		{
			m_Rd = GetComponent<Renderer>();
		}
		public void UpdateSelfParameters()
		{
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
			{
				mats[i].SetFloat("_HaloBloom", m_HaloBloom);
				mats[i].SetTexture("_SecondaryTex", m_SecondaryTex);
				mats[i].SetTextureScale("_SecondaryTex", m_SecondaryTexScale);
				mats[i].SetTexture("_StripeTex", m_StripeTex);
				mats[i].SetColor("_StripeColor", m_StripeColor);
				mats[i].SetFloat("_Blend2TexBloom", m_Blend2TexBloom);
				mats[i].SetFloat("_StripeBloom", m_StripeBloom);
			}
		}
		public void SetMaterialsFloat(string name, float f)
		{
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
				mats[i].SetFloat(name, f);
		}
		public void SetMaterialsVector(string name, Vector4 v)
		{
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
				mats[i].SetVector(name, v);
		}
		public void ApplyKeyword(string keyword, bool enable)
		{
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
			{
				if (enable)
					mats[i].EnableKeyword(keyword);
				else
					mats[i].DisableKeyword(keyword);
			}
		}
		public void ApplyShader(Shader sdr)
		{
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
				mats[i].shader = sdr;
		}
	}
}