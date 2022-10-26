using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Cube : MonoBehaviour
{
    public List<Renderer> TargetGameObjects = new List<Renderer>();
    public List<MeshRendererProperties> MeshRendererPropertiesList = new List<MeshRendererProperties>();
    public bool IsTarget = false;
    public List<Renderer> RenderersForRenderTextures = new List<Renderer>();
    public TrueHitController CurrentTrueHitController;
    public int Index = -1;
    public bool isFounded = false;
    public Bounds Bounds;
    public Bounds BoundsTarget;
    public Vector3 SphereScale = new Vector3(40, 40, 40);
    private void Awake()
    {
        MeshRendererPropertiesList = TargetGameObjects.Select(qq => qq.GetComponent<MeshRendererProperties>()).ToList();
        SetBounds();
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

        transform.position = Bounds.center;
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

        transform.position += new Vector3(1000, 1000, 1000);
        BoundsTarget = RenderersForRenderTextures[0].bounds;
        for (int i = 1; i < RenderersForRenderTextures.Count; i++)
        {
            BoundsTarget.Encapsulate(RenderersForRenderTextures[i].bounds);
        }
        M_TargetCamera.I.SetTargets(BoundsTarget);
    }
    private void SetBounds()
    {

        Bounds = MeshRendererPropertiesList[0].Renderer.bounds;
        for (int i = 1; i < MeshRendererPropertiesList.Count; i++)
        {
            Bounds.Encapsulate(MeshRendererPropertiesList[i].Renderer.bounds);
        }
    }

    public void Found()
    {
        isFounded = true;
        for (int i = 0; i < RenderersForRenderTextures.Count; i++)
        {
            Destroy(RenderersForRenderTextures[i].gameObject);
        }
    }
    public void Shake()
    {
        int _count = MeshRendererPropertiesList.Count;
        for (int i = 0; i < _count; i++)
        {
            MeshRendererProperties _meshRendererProperties = MeshRendererPropertiesList[i];
            _meshRendererProperties.Renderer.transform.DOKill();
            _meshRendererProperties.Renderer.transform.localEulerAngles = _meshRendererProperties.LocalEulerAngle;
            _meshRendererProperties.Renderer.transform.DOShakeRotation(0.5f,
                new Vector3(Random.Range(-6f, 6f), 0, Random.Range(-6f, 6f)), 150, 90, true);
        }
    }
    private List<Vector3> boundCorners = new List<Vector3>();
    public void CalculateSphereScale(Vector3 _targetPosition)
    {
        boundCorners.Clear();
        boundCorners.Add(new Vector3(Bounds.min.x, Bounds.min.y, Bounds.min.z));
        boundCorners.Add(new Vector3(Bounds.max.x, Bounds.min.y, Bounds.min.z));
        boundCorners.Add(new Vector3(Bounds.min.x, Bounds.max.y, Bounds.min.z));
        boundCorners.Add(new Vector3(Bounds.max.x, Bounds.max.y, Bounds.min.z));
        boundCorners.Add(new Vector3(Bounds.min.x, Bounds.min.y, Bounds.max.z));
        boundCorners.Add(new Vector3(Bounds.max.x, Bounds.min.y, Bounds.max.z));
        boundCorners.Add(new Vector3(Bounds.min.x, Bounds.max.y, Bounds.max.z));
        boundCorners.Add(new Vector3(Bounds.max.x, Bounds.max.y, Bounds.max.z));
        float _distance = 0;
        for (int i = 0; i < boundCorners.Count; i++)
        {
            float _currentDistance = Vector3.Distance(_targetPosition, boundCorners[i]);
            if (_currentDistance > _distance)
            {
                _distance = _currentDistance;
            }
        }
        _distance *= 2;
        SphereScale = new Vector3(_distance, _distance, _distance);
    }
}