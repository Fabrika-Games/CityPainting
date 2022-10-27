using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongParticle : MonoBehaviour
{
    public static WrongParticle I;
    [SerializeField]
    private ParticleSystem ParticleSystem;
    void Awake()
    {
        I = this;
    }

    private void OnEnable()
    {
        M_Observer.OnFalseHitAnimation += FalseHitAnimation;
    }
    private void OnDisable()
    {
        M_Observer.OnFalseHitAnimation -= FalseHitAnimation;
    }
    private void FalseHitAnimation(Cube _cube)
    {

        for (int i = 0; i < _cube.MeshRendererPropertiesList.Count; i++)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.position = _cube.MeshRendererPropertiesList[i].TopMidPoint;
            ParticleSystem.Emit(emitParams, 1);
        }
    }

}