using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugContainer : MonoBehaviour
{
    void Awake()
    {
        if (!M_Config.I.IsDebug)
        {
            Destroy(gameObject);
        }
    }
}