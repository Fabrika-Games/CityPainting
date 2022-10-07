using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_Data : MonoBehaviour
{
    public UnlockSystemData UnlockSystemData;

    private void Awake()
    {
        II = this;
    }

    public static M_Data II;

    public static M_Data I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Data").GetComponent<M_Data>();
            }

            return II;
        }
    }
}