using System;
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
    private void OnEnable()
    {
        M_Observer.OnGameCreate += GameCreate;
        M_Observer.OnGameReady += GameReady;
        M_Observer.OnGameStart += GameStart;
        M_Observer.OnGameFail += GameFail;
        M_Observer.OnGameComplete += GameComplete;
    }
    private void OnDisable()
    {
        M_Observer.OnGameCreate -= GameCreate;
        M_Observer.OnGameReady -= GameReady;
        M_Observer.OnGameStart -= GameStart;
        M_Observer.OnGameFail -= GameFail;
        M_Observer.OnGameComplete -= GameComplete;
    }
    private void GameReady()
    {
        GoToTarget(M_Level.I.CurrentLevel.BeginBounds);
    }
    private void GameCreate()
    {

    }
    private void GameStart()
    {
    }
    private void GameFail()
    {
    }
    private void GameComplete()
    {
        GoToTarget(M_Level.I.CurrentLevel.CurrentBounds);
        DOTween.To((xx) => { levelCompleteRotateSpeed = xx; }, 90, 4, 1.25f).SetEase(Ease.OutSine);
        // DOTween.To((xx) => { levelCompleteRotateSpeed = xx; }, 0, 180, 0.25f).SetEase(Ease.OutSine).OnComplete(() =>
        // {
        // });
    }

    private float levelCompleteRotateSpeed = 2;
    private void Update()
    {
        if (M_Observer.CurrentGameStatus == M_Observer.GameStatus.LevelComplete)
        {
            CameraPerspective.Rotate(new Vector2(levelCompleteRotateSpeed, 0));
        }
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