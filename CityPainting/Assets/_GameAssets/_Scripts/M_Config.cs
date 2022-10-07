using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Config : MonoBehaviour
{
    public bool IsDebug = false;
    public bool IsActiveSystem_MultiplyCollectedMoney = true;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }


    public static M_Config II;

    public static M_Config I
    {
        get
        {
            if (II == null)
            {
                GameObject _g = GameObject.Find("M_Config");
                if (_g != null)
                {
                    II = _g.GetComponent<M_Config>();
                }
            }

            return II;
        }
    }
}