using System.Collections;
using System.Collections.Generic;
using Exoa.Cameras;
using UnityEngine;

public class InteriorWalls : MonoBehaviour
{

    void Awake()
    {
        M_Camera.I.CameraPerspective.gameObject.GetComponent<CameraBounderies>().bounderiesCollider = GetComponent<Collider>();
    }


}