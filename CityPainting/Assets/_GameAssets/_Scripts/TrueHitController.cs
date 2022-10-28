using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class TrueHitController : MonoBehaviour
{
    public Collider Collider;

    public void Setup(Cube _currentCube, Vector3 _hitPoint)
    {
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            M_Observer.OnTrueHitAnimationStart?.Invoke();

            M_Camera.I.GoToTarget(_currentCube.Bounds);
            yield return new WaitForSeconds(0.125f);

            _currentCube.CalculateSphereScale(_hitPoint);
            Level _level = M_Level.I.CurrentLevel;
            _currentCube.CurrentTrueHitController = this;
            transform.SetParent(_currentCube.transform);
            transform.position = _hitPoint;
            transform.localScale = new Vector3(0, 0, 0);
            int _meshRendererPropertiesListCount = _currentCube.MeshRendererPropertiesList.Count;
            for (int i = 0; i < _meshRendererPropertiesListCount; i++)
            {
                // _currentCube.MeshRendererPropertiesList[i].transform.DOShakePosition(1f, new Vector3(0, Random.Range(0.1f, 0.25f), 0), 30).SetDelay(0.5f * i / _meshRendererPropertiesListCount).SetEase(Ease.InOutExpo);
                _currentCube.MeshRendererPropertiesList[i].Renderer.transform.DOMove(_currentCube.MeshRendererPropertiesList[i].Position + new Vector3(0, 3, 0), 0.3f).SetDelay(0.2f * i / _meshRendererPropertiesListCount).SetEase(Ease.OutBack);
            }
            yield return new WaitForSeconds(0.1f);
            List<ParticleTrigger> _particleTriggerList = new List<ParticleTrigger>();
            for (int i = 0; i < _meshRendererPropertiesListCount; i++)
            {
                ParticleTrigger _pt = M_Pool.I.GetFromPool<ParticleTrigger>();
                _pt.SetupParticleSystem(_currentCube.MeshRendererPropertiesList[i], Collider);
                _particleTriggerList.Add(_pt);
            }
            List<Material> _aniMats = new List<Material>();
            for (int i = 0; i < _meshRendererPropertiesListCount; i++)
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
            transform.DOScale(_currentCube.SphereScale, 1.5f).SetEase(Ease.Linear).OnUpdate(() =>
            {
                for (int i = 0; i < _aniMats.Count; i++)
                {
                    _aniMats[i].SetFloat("_Radius", Mathf.Clamp(transform.localScale.x, 1, 100));
                }
            }).OnComplete(() =>
            {
                StartCoroutine(Delay1());

                IEnumerator Delay1()
                {
                    for (int i = 0; i < _meshRendererPropertiesListCount; i++)
                    {
                        _currentCube.MeshRendererPropertiesList[i].Renderer.transform.DOMove(_currentCube.MeshRendererPropertiesList[i].Position, 0.4f).SetDelay(0.1f * i / _meshRendererPropertiesListCount).SetEase(Ease.OutBounce);
                    }
                    yield return new WaitForSeconds(0.5f);
                    for (int i = 0; i < _meshRendererPropertiesListCount; i++)
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
                    M_Observer.OnTrueHitAnimationComplete?.Invoke();
                    yield return new WaitForSeconds(1);
                    for (int i = 0; i < _particleTriggerList.Count; i++)
                    {
                        M_Pool.I.TakeToPool<ParticleTrigger>(_particleTriggerList[i]);
                    }
                    Destroy(gameObject);
                }
            });
        }
    }


    public void DeleteTrueHitController()
    {
    }
}