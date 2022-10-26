using System.Collections.Generic;
using UnityEngine;

namespace Exoa.Designer
{
	public static class GameObjectExtensions
	{

		public static void ApplyLayerRecursively(this GameObject go, string layer)
		{
			Transform t = go.transform;
			go.layer = LayerMask.NameToLayer(layer);
			for (int i = 0; i < t.childCount; i++)
			{
				t.GetChild(i).gameObject.ApplyLayerRecursively(layer);
			}
		}


		public static Bounds GetBoundsRecursive(this GameObject go)
		{
			Bounds b = new Bounds(go.transform.position, Vector3.zero);
			List<Renderer> rList = go.GetComponentsInChildrenRecursive<Renderer>();
			foreach (Renderer r in rList)
			{
				//Debug.Log("Added Renderer bounds " + r.bounds);
				b.Encapsulate(r.bounds);
			}
			return b;
		}


		public static List<T> GetComponentsInChildrenRecursive<T>(this GameObject topLevelGO) where T : Component
		{
			List<T> components = new List<T>();
			SearchForComponent_Helper<T>(topLevelGO, components);
			return (components);
		}

		private static void SearchForComponent_Helper<T>(GameObject _go, List<T> list) where T : Component
		{
			Transform t = _go.transform;
			int numChildren = t.childCount;

			if (numChildren > 0)
			{
				for (int i = 0; i < numChildren; i++)
				{
					Transform child = t.GetChild(i);
					SearchForComponent_Helper<T>(child.gameObject, list);
				}
			}

			T component = _go.GetComponent<T>(); //Something is going wrong here?

			if (component != null)
			{
				//Debug.Log("Adding " + component + " from " + _go.name);
				list.Add(component);
			}
		}

	}
}
