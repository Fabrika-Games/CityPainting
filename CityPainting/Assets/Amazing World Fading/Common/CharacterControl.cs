using UnityEngine;

namespace AmazingWorldFading
{
	public class CharacterControl : MonoBehaviour
	{
		public float m_MoveSpeed = 4f;
		public KeyCode m_ForwardButton = KeyCode.UpArrow;
		public KeyCode m_BackwardButton = KeyCode.DownArrow;
		public KeyCode m_RightButton = KeyCode.RightArrow;
		public KeyCode m_LeftButton = KeyCode.LeftArrow;
		public KeyCode m_UpButton = KeyCode.P;
		public KeyCode m_DownButton = KeyCode.L;
		Transform m_Character;

		void Start()
		{
			if (m_Character == null)
				m_Character = GetComponent<Transform>();
		}
		void Update()
		{
			Vector3 dir = Vector3.zero;
			Move(m_ForwardButton, ref dir, transform.forward);
			Move(m_BackwardButton, ref dir, -transform.forward);
			Move(m_RightButton, ref dir, transform.right);
			Move(m_LeftButton, ref dir, -transform.right);
			Move(m_UpButton, ref dir, transform.up);
			Move(m_DownButton, ref dir, -transform.up);
			m_Character.position += dir * m_MoveSpeed * Time.deltaTime;
		}
		void Move(KeyCode key, ref Vector3 moveTo, Vector3 dir)
		{
			if (Input.GetKey(key))
				moveTo = dir;
		}
	}
}