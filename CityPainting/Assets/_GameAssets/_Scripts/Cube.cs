using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public List<Renderer> TargetGameObjects = new List<Renderer>();

    [ContextMenu("RendererOpen")]
    void RendererOpen()
    {
        for (int i = 0; i < TargetGameObjects.Count; i++)
        {
            TargetGameObjects[i].enabled = true;
        }

        DestroyImmediate(gameObject);
    }
}