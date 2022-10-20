using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public List<Renderer> TargetGameObjects = new List<Renderer>();
    public List<MeshRendererProperties> MeshRendererPropertiesList = new List<MeshRendererProperties>();
    public bool IsTarget = false;
    public List<Renderer> RenderersForRenderTextures = new List<Renderer>();
    public TrueHitController CurrentTrueHitController;
    public Vector3 BoundsCenter;
    public int Index = -1;
    public bool isFounded = false;

    private void Awake()
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
        // for (int i = 0; i < MeshRendererPropertiesList.Count; i++)
        // {
        //     MeshRendererPropertiesList[i].Renderer.sharedMaterials = MeshRendererPropertiesList[i].ColoredMaterials;
        // }
    }

    public void MakeTarget()
    {
        IsTarget = true;
        Level _level = M_Level.I.CurrentLevel;
        Bounds _bounds = MeshRendererPropertiesList[0].Renderer.bounds;
        for (int i = 1; i < MeshRendererPropertiesList.Count; i++)
        {
            _bounds.Encapsulate(MeshRendererPropertiesList[i].Renderer.bounds);
        }

        transform.position = _bounds.center;
        if (RenderersForRenderTextures.Count == 0)
        {
            for (int i = 0; i < MeshRendererPropertiesList.Count; i++)
            {
                Renderer _renderer = Instantiate(MeshRendererPropertiesList[i].Renderer, transform);
                _renderer.gameObject.layer = LayerMask.NameToLayer("Cube");
                _renderer.transform.localPosition =
                    transform.InverseTransformPoint(MeshRendererPropertiesList[i].Renderer.transform.position);

                Material[] _materials = new Material[MeshRendererPropertiesList[i].MaterialIndexes.Count];
                for (int j = 0; j < _materials.Length; j++)
                {
                    _materials[j] = _level.DualMaterials[MeshRendererPropertiesList[i].MaterialIndexes[j]]
                        .ColoredMaterial;
                }

                _renderer.sharedMaterials = _materials;
                RenderersForRenderTextures.Add(_renderer);
            }
        }

        BoundsCenter = _bounds.center;
        transform.position += new Vector3(1000, 1000, 1000);
        _bounds = RenderersForRenderTextures[0].bounds;
        for (int i = 1; i < RenderersForRenderTextures.Count; i++)
        {
            _bounds.Encapsulate(RenderersForRenderTextures[i].bounds);
        }

        M_TargetCamera.I.SetTargets(_bounds);
    }

    public void Found()
    {
        isFounded = true;
        for (int i = 0; i < RenderersForRenderTextures.Count; i++)
        {
            Destroy(RenderersForRenderTextures[i].gameObject);
        }
    }
}