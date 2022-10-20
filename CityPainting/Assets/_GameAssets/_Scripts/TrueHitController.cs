using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrueHitController : MonoBehaviour
{
    public Collider Collider;

    public void Setup(Cube _currentCube, Vector3 _hitPoint)
    {
        Level _level = M_Level.I.CurrentLevel;
        _currentCube.CurrentTrueHitController = this;
        transform.SetParent(_currentCube.transform);
        transform.position = _hitPoint;
        transform.localScale = new Vector3(1, 0, 0);
        List<Material> _aniMats = new List<Material>();
        for (int i = 0; i < _currentCube.MeshRendererPropertiesList.Count; i++)
        {
            Renderer _r = _currentCube.MeshRendererPropertiesList[i].Renderer;
            Material[] _mats = new Material[_currentCube.MeshRendererPropertiesList[i].MaterialIndexes.Count];
            for (int j = 0; j < _mats.Length; j++)
            {
                _mats[j] = _level.DualMaterials[_currentCube.MeshRendererPropertiesList[i].MaterialIndexes[j]]
                    .AnimationMaterial;
                _aniMats.Add(_mats[j]);
            }

            _r.sharedMaterials = _mats;
            ParticleTrigger _pt = M_Pool.I.GetFromPool<ParticleTrigger>();
            _pt.SetupParticleSystem(_currentCube.MeshRendererPropertiesList[i], Collider);
        }

        for (int i = 0; i < _aniMats.Count; i++)
        {
            _aniMats[i].SetVector("_Position", _hitPoint);
        }


        transform.DOScale(new Vector3(30, 30, 30), 2.5f).SetEase(Ease.OutSine).OnUpdate(() =>
        {
            for (int i = 0; i < _aniMats.Count; i++)
            {
                _aniMats[i].SetFloat("_Radius", transform.localScale.x);
            }
        }).OnComplete(() =>
        {
            for (int i = 0; i < _currentCube.MeshRendererPropertiesList.Count; i++)
            {
                Renderer _r = _currentCube.MeshRendererPropertiesList[i].Renderer;
                Material[] _mats = new Material[_currentCube.MeshRendererPropertiesList[i].MaterialIndexes.Count];
                for (int j = 0; j < _mats.Length; j++)
                {
                    _mats[j] = _level.DualMaterials[_currentCube.MeshRendererPropertiesList[i].MaterialIndexes[j]]
                        .ColoredMaterial;
                }

                _r.sharedMaterials = _mats;
            }
        });
    }
}