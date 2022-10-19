using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.EventSystems;

// [RequireComponent(typeof(ParticleSystem))]
public class ParticleTrigger : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        // var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // sphere.transform.parent = ps.transform;
        // sphere.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);
        // sphere.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        // sphere.GetComponent<MeshRenderer>().material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
        //
        // var shape = ps.shape;
        // shape.enabled = false;
        //
        // var trigger = ps.trigger;
        // trigger.enabled = true;
        // trigger.SetCollider(0, sphere.GetComponent<Collider>());
    }



    List<ParticleSystem.Particle> enterList = new List<ParticleSystem.Particle>();


    void OnParticleTrigger()
    {
        enterList.Clear();
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enterList[i];
            p.startSize = 0.2f;
            p.velocity = new Vector3(Random.Range(-1f, 1f), 5, Random.Range(-1f, 1f));
            p.startLifetime = Random.Range(0.45f, 0.65f);
            p.remainingLifetime = p.startLifetime;
            enterList[i] = p;
        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

    }
}