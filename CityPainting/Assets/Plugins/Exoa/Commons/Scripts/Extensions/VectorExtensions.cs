using UnityEngine;

public static class VectorExtensions
{


	public static Vector3 SetY(this Vector3 v, float y)
	{
		Vector3 newv = v;
		newv.y = y;
		return newv;
	}


}
