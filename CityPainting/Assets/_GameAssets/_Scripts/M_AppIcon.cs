using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class M_AppIcon : MonoBehaviour
{
    // private void OnApplicationFocus(bool hasFocus)
    // {
    //     if (hasFocus == true)
    //     {
    //         return;
    //     }
    //
    //     AppIconChanger.iOS.SetAlternateIconName("icon_" + Random.Range(0, 4).ToString("00"));
    // }
    //
    // private void OnApplicationQuit()
    // {
    // }

    private void Awake()
    {
        II = this;
    }

    public static M_AppIcon II;

    public static M_AppIcon I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_AppIcon").GetComponent<M_AppIcon>();
            }

            return II;
        }
    }
}