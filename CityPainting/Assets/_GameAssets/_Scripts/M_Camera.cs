using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class M_Camera : MonoBehaviour
{
    public int PresetIndex = 0;
    public List<PresetCMT> Presets;
    public CameraMultiTarget CameraMultiTarget;
    public Transform CameraTargetCont;

    void Awake()
    {
        II = this;
        M_Observer.OnGameReady += GameReady;
        M_Observer.OnGameStart += GameStart;
        M_Observer.OnGameComplete += GameComplete;
    }

    private void OnDestroy()
    {
        M_Observer.OnGameReady -= GameReady;
        M_Observer.OnGameStart -= GameStart;
        M_Observer.OnGameComplete -= GameComplete;
    }

    private void GameComplete()
    {
        StartCoroutine(GameComplete());

        IEnumerator GameComplete()
        {
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(0.75f);
            PresetIndex = 1;
            CameraTargetCont.DOMove(new Vector3(0, 15, 0), 1.0f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(1);
        }
    }

    private void GameReady()
    {
        PresetIndex = 0;
        CameraTargetCont.DOMove(new Vector3(), 0.5f);
    }

    private void GameStart()
    {
    }

    private PositionAndRotation targetPositionAndRotation;

    private void LateUpdate()
    {
        RefreshPositionAndRotation();
    }

    private float velA;
    private float velB;
    private float velC;
    private float velD;
    private float velE;
    private float velF;
    private float velG;
    private float velH;

    private void RefreshPositionAndRotation()
    {
        CameraMultiTarget.Pitch = Mathf.SmoothDamp(CameraMultiTarget.Pitch, Presets[PresetIndex].Pitch, ref velA,
            Presets[PresetIndex].MoveSmoothTime);
        CameraMultiTarget.Roll = Mathf.SmoothDamp(CameraMultiTarget.Roll, Presets[PresetIndex].Roll, ref velB,
            Presets[PresetIndex].MoveSmoothTime);
        CameraMultiTarget.Yaw = Mathf.SmoothDamp(CameraMultiTarget.Yaw, Presets[PresetIndex].Yaw, ref velC,
            Presets[PresetIndex].MoveSmoothTime);
        CameraMultiTarget.PaddingDown = Mathf.SmoothDamp(CameraMultiTarget.PaddingDown,
            Presets[PresetIndex].PaddingDown, ref velD, Presets[PresetIndex].MoveSmoothTime);
        CameraMultiTarget.PaddingLeft = Mathf.SmoothDamp(CameraMultiTarget.PaddingLeft,
            Presets[PresetIndex].PaddingLeft, ref velE, Presets[PresetIndex].MoveSmoothTime);
        CameraMultiTarget.PaddingRight = Mathf.SmoothDamp(CameraMultiTarget.PaddingRight,
            Presets[PresetIndex].PaddingRight, ref velF, Presets[PresetIndex].MoveSmoothTime);
        CameraMultiTarget.PaddingUp = Mathf.SmoothDamp(CameraMultiTarget.PaddingUp, Presets[PresetIndex].PaddingUp,
            ref velG, Presets[PresetIndex].MoveSmoothTime);

        targetPositionAndRotation = CameraMultiTarget.TargetPositionAndRotation();
        CameraMultiTarget.transform.position = targetPositionAndRotation.Position;
        CameraMultiTarget.transform.rotation = targetPositionAndRotation.Rotation;
    }

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