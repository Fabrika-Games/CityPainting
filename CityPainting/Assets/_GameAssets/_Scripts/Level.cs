using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject MeshRenderersContainer;
    public GameObject Cubes;
    public Material WhiteMaterial;


    private void Start()
    {
        MeshRenderer[] _meshRenderers = MeshRenderersContainer.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            MeshRenderer _mr = _meshRenderers[i];
            MeshRendererProperties _mrp = _mr.gameObject.AddComponent<MeshRendererProperties>();
            _mrp.ColoredMaterials = _mr.sharedMaterials;
            Material[] _whiteMaterials = new Material[_mr.sharedMaterials.Length];
            for (int j = 0; j < _mr.sharedMaterials.Length; j++)
            {
                _whiteMaterials[j] = WhiteMaterial;
            }

            _mr.sharedMaterials = _whiteMaterials;
        }

        Collider[] _colliders = MeshRenderersContainer.GetComponentsInChildren<Collider>();
        for (int i = 0; i < _colliders.Length; i++)
        {
            Destroy(_colliders[i]);
        }

        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].gameObject.AddComponent<MeshCollider>();
        }
    }
}