using System;
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

    public static GameObject PickObject(Vector2 screenPos, out Vector3 hitPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            hitPoint = hit.point;
            return hit.collider.gameObject;
        }

        hitPoint = Vector3.zero;
        return null;
    }
    public static GameObject PickObject(Vector2 screenPos, out RaycastHit _raycastHit)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            _raycastHit = hit;
            return hit.collider.gameObject;
        }

        _raycastHit = new RaycastHit();
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

    public static Texture2D ConvertToGrayscale(Texture2D graph, float value = 1)
    {
        Texture2D grayImg = new Texture2D(graph.width, graph.height);
        grayImg.SetPixels(graph.GetPixels());
        grayImg.Apply();

        Color32[] pixels = grayImg.GetPixels32();
        for (int x = 0; x < grayImg.width; x++)
        {
            for (int y = 0; y < grayImg.height; y++)
            {
                Color32 pixel = pixels[x + y * grayImg.width];
                int p = ((256 * 256 + pixel.r) * 256 + pixel.b) * 256 + pixel.g;
                int b = p % 256;
                p = Mathf.FloorToInt(p / 256);
                int g = p % 256;
                p = Mathf.FloorToInt(p / 256);
                int r = p % 256;
                float l = (0.2126f * r / 255f) + 0.7152f * (g / 255f) + 0.0722f * (b / 255f);
                l = l * value;
                Color c = new Color(l, l, l, 1);
                grayImg.SetPixel(x, y, c);
            }
        }

        grayImg.Apply();
        return grayImg;
    }


}