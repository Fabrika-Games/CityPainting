using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class M_Prefabs : MonoBehaviour
{
    public TrueHitController TrueHitControllerPrefab;
    public Material AnimationMaterialPrefab;
    public Material CubeBlackWhite;
    private void Awake()
    {
        II = this;
    }

    public static M_Prefabs II;

    public static M_Prefabs I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Prefabs").GetComponent<M_Prefabs>();
            }

            return II;
        }
    }
}