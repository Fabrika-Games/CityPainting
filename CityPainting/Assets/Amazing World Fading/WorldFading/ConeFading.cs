//using UnityEngine;
//using System.Collections;

//public class ConeFading : MonoBehaviour
//{
//	public Light m_SpotLight;
//	KeyCode m_ForwardButton = KeyCode.UpArrow;
//	KeyCode m_BackwardButton = KeyCode.DownArrow;
//	KeyCode m_RightButton = KeyCode.RightArrow;
//	KeyCode m_LeftButton = KeyCode.LeftArrow;
//	KeyCode m_UpButton = KeyCode.P;
//	KeyCode m_DownButton = KeyCode.L;

//	void Start ()
//	{}
//	void Update ()
//	{
//		Vector3 position = m_SpotLight.transform.position;
//		Vector3 normal = m_SpotLight.transform.forward;
//		float radius = m_SpotLight.range * Mathf.Tan(m_SpotLight.spotAngle * Mathf.Deg2Rad * 0.5f);
//		float height = m_SpotLight.range;

//		Shader.SetGlobalVector("_ConePos", position);
//		Shader.SetGlobalVector("_ConeNormal", normal);
//		Shader.SetGlobalFloat("_ConeRadius", radius);
//		Shader.SetGlobalFloat("_ConeHeight", height);

		// translation main character
//		{
//			Vector3 dir = Vector3.zero;
//			Move (m_ForwardButton, ref dir, transform.forward);
//			Move (m_BackwardButton, ref dir, -transform.forward);
//			Move (m_RightButton, ref dir, transform.right);
//			Move (m_LeftButton, ref dir, -transform.right);
//			Move (m_UpButton, ref dir, transform.up);
//			Move (m_DownButton, ref dir, -transform.up);
//			m_SpotLight.transform.position += dir * 4f * Time.deltaTime;
//		}
//	}
//	void Move (KeyCode key, ref Vector3 moveTo, Vector3 dir)
//	{
//		if (Input.GetKey (key))
//			moveTo = dir;
//	}
//}
