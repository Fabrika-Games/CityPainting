using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject MeshRenderersContainer;
    public GameObject Cubes;
    public Material WhiteMaterial;
    public Cube[] AllCubes;

    private void Start()
    {
        AllCubes = Cubes.GetComponentsInChildren<Cube>();

        MeshRenderer[] _meshRenderers = MeshRenderersContainer.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            MeshRenderer _mr = _meshRenderers[i];
            MeshRendererProperties _mrp = _mr.gameObject.AddComponent<MeshRendererProperties>();
            _mrp.ColoredMaterials = _mr.sharedMaterials;
            _mrp.Renderer = _mr;
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

        for (int i = 0; i < AllCubes.Length; i++)
        {
            for (int j = 0; j < AllCubes[i].TargetGameObjects.Count; j++)
            {
                AllCubes[i].TargetGameObjects[j].GetComponent<MeshRendererProperties>().CurrentCube = AllCubes[i];
            }
        }
    }
}