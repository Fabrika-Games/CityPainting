using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject MeshRenderersContainer;
    public GameObject Cubes;
    public Cube[] AllCubes;
    public int TargetIndex = -1;
    public List<DualMaterial> DualMaterials = new List<DualMaterial>();

    private void Awake()
    {
        AllCubes = Cubes.GetComponentsInChildren<Cube>();
        MeshRenderer[] _meshRenderers = MeshRenderersContainer.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            MeshRenderer _mr = _meshRenderers[i];
            MeshRendererProperties _mrp = _mr.gameObject.AddComponent<MeshRendererProperties>();
            for (int j = 0; j < _mr.sharedMaterials.Length; j++)
            {
                Material _m = _mr.sharedMaterials[j];
                if (DualMaterials.Any(qq => qq.ColoredMaterial == _m) == false)
                {
                    DualMaterial _dualMaterial = new DualMaterial();
                    _dualMaterial.Index = DualMaterials.Count;
                    _dualMaterial.ColoredMaterial = _m;
                    _dualMaterial.WhiteMaterial = Instantiate(_m);
                    _dualMaterial.WhiteMaterial.name += "_White";
                    if (_dualMaterial.WhiteMaterial.mainTexture != null)
                    {
                        Texture2D _grayTexture =
                            CihanUtility.ConvertToGrayscale((Texture2D)_dualMaterial.WhiteMaterial.mainTexture);
                        _dualMaterial.WhiteMaterial.mainTexture = _grayTexture;
                    }

                    _dualMaterial.AnimationMaterial = Instantiate(M_Prefabs.I.AnimationMaterialPrefab);
                    _dualMaterial.AnimationMaterial.SetTexture("_Set1_albedo", _dualMaterial.WhiteMaterial.mainTexture);
                    _dualMaterial.AnimationMaterial.SetTexture("_Set2_albedo",
                        _dualMaterial.ColoredMaterial.mainTexture);

                    _dualMaterial.AnimationMaterial.SetColor("_Set1_albedo_tint", _dualMaterial.WhiteMaterial.color);
                    _dualMaterial.AnimationMaterial.SetColor("_Set2_albedo_tint", _dualMaterial.ColoredMaterial.color);

                    _dualMaterial.AnimationMaterial.SetFloat("_Set1_metallic_multiplier",
                        _dualMaterial.WhiteMaterial.GetFloat("_Metallic"));
                    _dualMaterial.AnimationMaterial.SetFloat("_Set2_metallic_multiplier",
                        _dualMaterial.ColoredMaterial.GetFloat("_Metallic"));

                    _dualMaterial.AnimationMaterial.SetFloat("_Set1_smoothness",
                        _dualMaterial.WhiteMaterial.GetFloat("_Glossiness"));
                    _dualMaterial.AnimationMaterial.SetFloat("_Set2_smoothness",
                        _dualMaterial.ColoredMaterial.GetFloat("_Glossiness"));


                    // ("Set1_albedo_tint", Color) 
                    // _Set1_normal("Set1_normal", 2D) 
                    // _Set1_emission("Set1_emission", 2D) 
                    // _Set1_emission_tint("Set1_emission_tint", Color) 
                    // _Set1_metallic("Set1_metallic", 2D) 
                    // _Set1_tiling("Set1_tiling", Vector) 
                    // _Set1_offset("Set1_offset", Vector) 
                    // _Set2_normal("Set2_normal", 2D) 
                    // _Set2_emission("Set2_emission", 2D) 
                    // _Set2_emission_tint("Set2_emission_tint", Color) = (1, 1, 1, 1)
                    // _Set2_metallic("Set2_metallic", 2D) 
                    // _Set2_tiling("Set2_tiling", Vector) 
                    // _Set2_offset("Set2_offset", Vector) 


                    DualMaterials.Add(_dualMaterial);
                }
            }

            _mrp.Renderer = _mr;
            Material[] _whiteMaterials = new Material[_mr.sharedMaterials.Length];
            for (int j = 0; j < _mr.sharedMaterials.Length; j++)
            {
                _whiteMaterials[j] = GetWhiteMaterial(_mr.sharedMaterials[j], out int _index);
                _mrp.MaterialIndexes.Add(_index);
            }

            _mr.sharedMaterials = _whiteMaterials;
        }

        Collider[] _colliders = MeshRenderersContainer.GetComponentsInChildren<Collider>();
        for (int i = 0; i < _colliders.Length; i++)
        {
            Destroy(_colliders[i]);
        }

        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].gameObject.AddComponent<MeshCollider>();
        }

        for (int i = 0; i < AllCubes.Length; i++)
        {
            for (int j = 0; j < AllCubes[i].TargetGameObjects.Count; j++)
            {
                AllCubes[i].TargetGameObjects[j].GetComponent<MeshRendererProperties>().CurrentCube = AllCubes[i];
            }
        }
    }

    private Material GetWhiteMaterial(Material sharedMaterial)
    {
        Material m = DualMaterials.FirstOrDefault(qq => qq.ColoredMaterial == sharedMaterial).WhiteMaterial;
        return m;
    }

    private Material GetWhiteMaterial(Material sharedMaterial, out int _index)
    {
        DualMaterial _dualMaterial = DualMaterials.FirstOrDefault(qq => qq.ColoredMaterial == sharedMaterial);
        Material m = _dualMaterial.WhiteMaterial;
        _index = _dualMaterial.Index;
        return m;
    }

    private void OnEnable()
    {
        FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
    }

    private void OnDisable()
    {
        FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
    }

    private void FingerGestures_OnFingerTap(int fingerındex, Vector2 fingerpos, int tapcount)
    {
        if (fingerındex != 0)
        {
            return;
        }

        if (tapcount == 2)
        {
            GameObject _pickObject = CihanUtility.PickObject(fingerpos, out Vector3 hitPoint);
            if (_pickObject != null)
            {
                if (
                    _pickObject.TryGetComponent(out MeshRendererProperties _mrp) &&
                    _mrp.CurrentCube != null &&
                    _mrp.CurrentCube.IsTarget == true &&
                    _mrp.CurrentCube.CurrentTrueHitController == null)
                {
                    TrueHitController _trueHitController = Instantiate(M_Prefabs.I.TrueHitControllerPrefab);
                    _trueHitController.Setup(_mrp.CurrentCube, hitPoint);
                }
            }
        }
    }

    private void Start()
    {
        TargetCubeChange(0);
    }

    public void TargetCubeChange(int _index = -1)
    {
        if (_index == -1)
        {
            return;
        }

        TargetIndex = _index;
        for (int i = 0; i < AllCubes.Length; i++)
        {
            AllCubes[i].IsTarget = false;
        }

        AllCubes[_index].MakeTarget();
    }
}

[System.Serializable]
public struct DualMaterial
{
    public Material ColoredMaterial;
    public Material WhiteMaterial;
    public Material AnimationMaterial;
    public int Index;
}