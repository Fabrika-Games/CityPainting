using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererProperties : MonoBehaviour
{
    // public Material[] ColoredMaterials;
    public Cube CurrentCube;
    public MeshRenderer Renderer;
    public Vector3 LocalEulerAngle;
    public List<int> MaterialIndexes = new List<int>();
}