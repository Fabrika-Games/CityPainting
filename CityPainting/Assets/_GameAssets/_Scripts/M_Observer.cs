using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Observer : MonoBehaviour
{
    public static Action OnGameCreate;
    public static Action OnGameReady;
    public static Action OnGameStart;
    public static Action OnGameFail;
    public static Action OnGameComplete;
    public static Action OnGameRetry;
    public static Action OnGameContinue;
    public static Action OnGameNextLevel;
    public static Action<Cube> OnFalseHitAnimation;
    public static Action OnTrueHitAnimationStart;
    public static Action OnTrueHitAnimationComplete;

    public static GameStatus CurrentGameStatus;

    public enum GameStatus
    {
        MainMenu,
        InGame,
        LevelComplete,
        LevelFail

    }


    private void Awake()
    {
        II = this;
        OnGameCreate += GameCreate;
        OnGameReady += GameReady;
        OnGameStart += GameStart;
        OnGameFail += GameFail;
        OnGameComplete += GameComplete;
        OnGameRetry += GameRetry;
        OnGameContinue += GameContinue;
        OnGameNextLevel += GameNextLevel;
    }

    private void Start()
    {
        OnGameCreate?.Invoke();
    }

    private void OnDestroy()
    {
        OnGameCreate -= GameCreate;
        OnGameReady -= GameReady;
        OnGameStart -= GameStart;
        OnGameFail -= GameFail;
        OnGameComplete -= GameComplete;
        OnGameRetry -= GameRetry;
        OnGameContinue -= GameContinue;
        OnGameNextLevel -= GameNextLevel;
    }


    private void GameCreate()
    {
        //print("GameCreate");
        CurrentGameStatus = GameStatus.MainMenu;
    }

    private void GameReady()
    {
        //print("GameReady");
        CurrentGameStatus = GameStatus.MainMenu;
    }

    private void GameStart()
    {
        //print("GameStart");
        CurrentGameStatus = GameStatus.InGame;
    }

    private void GameFail()
    {
        //print("GameFail");
        CurrentGameStatus = GameStatus.LevelFail;
    }

    private void GameComplete()
    {
        //print("GameComplete");
        CurrentGameStatus = GameStatus.LevelComplete;
    }

    private void GameRetry()
    {
        //print("GameRetry");
    }

    private void GameContinue()
    {
        //print("GameContinue");
    }

    private void GameNextLevel()
    {
        //print("GameNextLevel");
    }

    public static M_Observer II;

    public static M_Observer I
    {
        get
        {
            if (II == null)
            {
                GameObject _g = GameObject.Find("M_Game");
                if (_g != null)
                {
                    II = _g.GetComponent<M_Observer>();
                }
            }

            return II;
        }
    }


}