using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MeshRendererProperties : MonoBehaviour
{
    // public Material[] ColoredMaterials;
    public Cube CurrentCube;
    public MeshRenderer Renderer;
    public Vector3 LocalEulerAngle;
    public List<int> MaterialIndexes = new List<int>();
    [FormerlySerializedAs("TopMidpoint")]
    public Vector3 TopMidPoint;
}