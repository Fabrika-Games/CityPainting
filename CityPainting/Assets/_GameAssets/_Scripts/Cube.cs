using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public List<Renderer> TargetGameObjects = new List<Renderer>();
    public List<MeshRendererProperties> MeshRendererPropertiesList = new List<MeshRendererProperties>();

    private void Start()
    {
        MeshRendererPropertiesList = TargetGameObjects.Select(qq => qq.GetComponent<MeshRendererProperties>()).ToList();
    }

    [ContextMenu("RendererOpen")]
    void RendererOpen()
    {
        for (int i = 0; i < TargetGameObjects.Count; i++)
        {
            TargetGameObjects[i].enabled = true;
        }

        DestroyImmediate(gameObject);
    }

    [ContextMenu("Colorize")]
    public void Colorize()
    {
        for (int i = 0; i < MeshRendererPropertiesList.Count; i++)
        {
            MeshRendererPropertiesList[i].Renderer.sharedMaterials = MeshRendererPropertiesList[i].ColoredMaterials;
        }
    }
}