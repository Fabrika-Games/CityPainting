using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Exoa.Cameras;
using UnityEngine;

public class M_Camera : MonoBehaviour
{
    public CameraPerspective CameraPerspective;

    void Awake()
    {
        II = this;
    }

    public void GoToTarget(Bounds _b)
    {
        CameraPerspective.FocusCameraOnGameObject(_b);
    }

    // public void GoToTarget(Vector3 _pos, float _duration = 0.5f)
    // {
    //     CameraPerspective.FocusCameraOnGameObject();
    // }


    public static M_Camera II;

    public static M_Camera I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Camera")?.GetComponent<M_Camera>();
            }

            return II;
        }
    }
}


[System.Serializable]
public class PresetCMT
{
    public string Name = "PresetName";
    public float Pitch;
    public float Yaw;
    public float Roll;
    public float PaddingLeft;
    public float PaddingRight;
    public float PaddingUp;
    public float PaddingDown;
    public float MoveSmoothTime = 0.19f;
}