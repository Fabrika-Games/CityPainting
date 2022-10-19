using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetRawImage : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;

    void OnEnable()
    {
        rawImage.texture = M_TargetCamera.I.CurrentCamera.targetTexture;
    }
}