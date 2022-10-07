using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ActivateIfPlatform : MonoBehaviour
{
	public List<RuntimePlatform> platforms = new List<RuntimePlatform>();

	private void Awake()
	{
		bool shouldBeActive = platforms.Contains(RuntimePlatformUtility.GetPlatform());
		gameObject.SetActive(shouldBeActive);
	}
}
