using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRandomPosition : MonoBehaviour
{
    void Start()
    {
        transform.position = Random.rotation.eulerAngles.normalized * 2;
    }
}