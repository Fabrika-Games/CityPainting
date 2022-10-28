using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    public GameObject MeshRenderersContainer;
    public GameObject Cubes;
    public Cube[] AllCubes;
    public int TargetIndex = -1;
    public List<DualMaterial> DualMaterials = new List<DualMaterial>();
    public int CubeCount;
    public int TrueHitCount = 0;
    public Bounds CurrentBounds;
    public Bounds BeginBounds = new Bounds(Vector3.zero, new Vector3(5, 5, 5));

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
                            CihanUtility.ConvertToGrayscale((Texture2D)_dualMaterial.WhiteMaterial.mainTexture, 1.25f);
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


                    DualMaterials.Add(_dualMaterial);
                }
            }

            _mrp.Renderer = _mr;
            _mrp.LocalEulerAngle = _mr.transform.localEulerAngles;
            _mrp.Position = _mr.transform.position;
            _mrp.TopMidPoint = new Vector3(_mr.bounds.center.x, _mr.bounds.max.y, _mr.bounds.center.z);
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
            AllCubes[i].Index = i;
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
        M_Observer.OnTrueHitAnimationComplete += TrueHitAnimationComplete;
        M_Observer.OnTrueHitAnimationStart += TrueHitAnimationStart;
    }

    private void OnDisable()
    {
        FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
        M_Observer.OnTrueHitAnimationComplete -= TrueHitAnimationComplete;
        M_Observer.OnTrueHitAnimationStart -= TrueHitAnimationStart;
    }

    private void TrueHitAnimationStart()
    {
        TrueHitCount++;
    }

    private void TrueHitAnimationComplete()
    {
        Cube _cube = AllCubes.Where(qq => qq.IsTarget == false)?.OrderBy(qq => Random.Range(0, 1f))?.FirstOrDefault();
        if (_cube != null)
        {
            TargetCubeChange(_cube.Index);
        }

        if (IsLevelComplete())
        {
            M_Observer.OnGameComplete?.Invoke();
        }
    }

    private void FingerGestures_OnFingerTap(int fingerındex, Vector2 fingerpos, int tapcount)
    {
        if (fingerındex != 0)
        {
            return;
        }

        if (M_Observer.CurrentGameStatus != M_Observer.GameStatus.InGame)
        {
            return;
        }

        if (tapcount == 1)
        {
            GameObject _pickObject = CihanUtility.PickObject(fingerpos, out Vector3 hitPoint);
            if (_pickObject != null)
            {
                if (
                    _pickObject.TryGetComponent(out MeshRendererProperties _mrp) &&
                    _mrp.CurrentCube != null &&
                    _mrp.CurrentCube.IsTarget == true &&
                    _mrp.CurrentCube.isFounded == false &&
                    _mrp.CurrentCube.CurrentTrueHitController == null)
                {
                    TrueHitController _trueHitController = Instantiate(M_Prefabs.I.TrueHitControllerPrefab);
                    _trueHitController.Setup(_mrp.CurrentCube, hitPoint);
                }
                else if (
                    _pickObject.TryGetComponent(out MeshRendererProperties _mrp2) &&
                    _mrp2.CurrentCube != null &&
                    _mrp2.CurrentCube.IsTarget == false &&
                    _mrp2.CurrentCube.isFounded == false &&
                    _mrp2.CurrentCube.CurrentTrueHitController == null)
                {
                    // M_Camera.I.GoToTarget(_mrp2.CurrentCube.Bounds);
                    _mrp2.CurrentCube.Shake();
                    M_Observer.OnFalseHitAnimation?.Invoke(_mrp2.CurrentCube);
                }
            }
        }
    }

    public void TargetCubeFound(int _index = -1)
    {
        if (_index == -1)
        {
            return;
        }

        AllCubes[_index].Found();
    }

    private void Start()
    {
        for (int i = 0; i < AllCubes.Length; i++)
        {
            AllCubes[i].IsTarget = false;
        }

        TargetCubeChange(0);
        M_Observer.OnGameReady?.Invoke();
    }

    public void TargetCubeChange(int _index = -1)
    {
        if (_index == -1)
        {
            return;
        }

        TargetCubeFound(TargetIndex);
        TargetIndex = _index;
        AllCubes[_index].MakeTarget();
    }


    [ContextMenu("DeleteNullGameObject")]
    public void DeleteNullGameObject()
    {
#if UNITY_EDITOR
        Transform[] _transforms = MeshRenderersContainer.GetComponentsInChildren<Transform>();
        for (int i = _transforms.Length - 1; i >= 0; i--)
        {
            if (_transforms[i].GetComponent<MeshRenderer>() == null && _transforms[i].childCount == 0)
            {
                DestroyImmediate(_transforms[i].gameObject);
            }
        }

        _transforms = MeshRenderersContainer.GetComponentsInChildren<Transform>();
        for (int i = _transforms.Length - 1; i >= 0; i--)
        {
            if (!(UnityEditor.PrefabUtility.GetPrefabParent(_transforms[i].gameObject) != null &&
                  UnityEditor.PrefabUtility.GetPrefabObject(_transforms[i].gameObject) != null))
            {
                if (_transforms[i].GetComponents<Collider>().Length > 0)
                {
                    for (int j = _transforms[i].GetComponents<Collider>().Length - 1; j >= 0; j--)
                    {
                        DestroyImmediate(_transforms[i].GetComponents<Collider>()[j]);
                    }
                }
            }
        }

        UnityEditor.EditorUtility.SetDirty(gameObject.GetComponentInParent<Level>());
#endif
    }


    [ContextMenu("SetCubeCount")]
    public void SetCubeCount()
    {
        CubeCount = Cubes.GetComponentsInChildren<Cube>().Length;
        gameObject.name += CubeCount.ToString("000");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    [ContextMenu("SetCurrentBounds")]
    public void SetCurrentBounds()
    {
        GetComponentInChildren<InteriorWalls>().GetComponent<Collider>().enabled = true;
        CurrentBounds = GetComponentInChildren<InteriorWalls>().GetComponent<Collider>().bounds;
        GetComponentInChildren<InteriorWalls>().GetComponent<Collider>().enabled = false;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }


    public bool IsLevelComplete()
    {
        return TrueHitCount == CubeCount;
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