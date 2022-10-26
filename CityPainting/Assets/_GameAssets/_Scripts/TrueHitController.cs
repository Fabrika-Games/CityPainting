using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TrueHitController : MonoBehaviour
{
    public Collider Collider;
    public void Setup(Cube _currentCube, Vector3 _hitPoint)
    {
        _currentCube.CalculateSphereScale(_hitPoint);
        M_Camera.I.GoToTarget(_currentCube.Bounds);
        Level _level = M_Level.I.CurrentLevel;
        _currentCube.CurrentTrueHitController = this;
        transform.SetParent(_currentCube.transform);
        transform.position = _hitPoint;
        transform.localScale = new Vector3(0, 0, 0);


        List<ParticleTrigger> _particleTriggerList = new List<ParticleTrigger>();
        for (int i = 0; i < _currentCube.MeshRendererPropertiesList.Count; i++)
        {


            ParticleTrigger _pt = M_Pool.I.GetFromPool<ParticleTrigger>();
            _pt.SetupParticleSystem(_currentCube.MeshRendererPropertiesList[i], Collider);
            _particleTriggerList.Add(_pt);
        }

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
        }

        for (int i = 0; i < _aniMats.Count; i++)
        {
            _aniMats[i].SetVector("_Position", _hitPoint);
            _aniMats[i].SetFloat("_Radius", 1);
        }

        transform.DOScale(_currentCube.SphereScale, 4f).SetEase(Ease.OutSine).OnUpdate(() =>
        {
            for (int i = 0; i < _aniMats.Count; i++)
            {
                _aniMats[i].SetFloat("_Radius", Mathf.Clamp(transform.localScale.x, 1, 100));
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

            for (int i = 0; i < _particleTriggerList.Count; i++)
            {
                M_Pool.I.TakeToPool<ParticleTrigger>(_particleTriggerList[i]);
            }

            M_Observer.OnTrueHitAnimationComplete?.Invoke();

            Destroy(gameObject);
        });

    }

    public void DeleteTrueHitController()
    {
    }


}