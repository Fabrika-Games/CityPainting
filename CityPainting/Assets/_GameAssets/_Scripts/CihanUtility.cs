using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CihanUtility
{
    public static Vector3 GetWorldPos(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // we solve for intersection with y = 0 plane
        float t = -ray.origin.z / ray.direction.z;

        return ray.GetPoint(t);
    }

    // Return the GameObject at the given screen position, or null if no valid object was found
    public static GameObject PickObject(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    public static GameObject PickObject(Vector2 screenPos, LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    public static Transform FindInAllChild(this Transform transform, string _name)
    {
        Transform[] _transforms = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < _transforms.Length; i++)
        {
            if (_transforms[i].name == _name)
            {
                return _transforms[i];
            }
        }

        return null;
    }
}