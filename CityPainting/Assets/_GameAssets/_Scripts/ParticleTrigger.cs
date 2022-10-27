using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

// [RequireComponent(typeof(ParticleSystem))]
public class ParticleTrigger : MonoBehaviour
{
    private MeshRendererProperties CurrentMeshrendererProperties;
    [SerializeField] private ParticleSystem ps;

    List<ParticleSystem.Particle> enterList = new List<ParticleSystem.Particle>();


    public void SetupParticleSystem(MeshRendererProperties _currentMeshrendererProperties, Collider _colldier)
    {
        ParticleSystem.TriggerModule _triggerModule = ps.trigger;
        _triggerModule.SetCollider(0, _colldier);
        ParticleSystem.ShapeModule _shapeModule = ps.shape;
        _shapeModule.meshRenderer = _currentMeshrendererProperties.Renderer;
        ParticleSystem.EmissionModule _emissionModule = ps.emission;
        _emissionModule.SetBursts(new ParticleSystem.Burst[]
            { new ParticleSystem.Burst(0.0f, _currentMeshrendererProperties.Renderer.bounds.size.magnitude * 15) });
        ps.Play();
    }

    void OnParticleTrigger()
    {
        enterList.Clear();
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enterList[i];
            p.startSize = 0.75f;
            p.velocity = new Vector3(Random.Range(-1f, 1f), 5, Random.Range(-1f, 1f));
            p.startLifetime = Random.Range(0.45f, 0.65f);
            p.remainingLifetime = p.startLifetime;
            enterList[i] = p;
        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);
    }

    private void OnDisable()
    {
        CurrentMeshrendererProperties = null;
    }
}