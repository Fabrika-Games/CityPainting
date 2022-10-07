using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class M_Money : MonoBehaviour
{
    public static Action<int> TotalMoneyChanged;
    public static Action<Transform> OnNoMoney;
    public static Action OnMoneyDecrease;

    public static int TotalMoney
    {
        set { PlayerPrefs.SetInt("TotalMoney", value); }
        get { return PlayerPrefs.GetInt("TotalMoney", 0); }
    }

    [HideInInspector] public int MoneyCollectedInTheGame;

    private void Awake()
    {
        II = this;
        TotalMoney = TotalMoney;

        M_Observer.OnGameCreate += GameCreate;
        M_Observer.OnGameReady += GameReady;
        M_Observer.OnGameStart += GameStart;
        M_Observer.OnGameFail += GameFail;
        M_Observer.OnGameComplete += GameComplete;
        M_Observer.OnGameRetry += GameRetry;
        M_Observer.OnGameContinue += GameContinue;
        M_Observer.OnGameNextLevel += GameNextLevel;
    }


    private void OnDestroy()
    {
        M_Observer.OnGameCreate -= GameCreate;
        M_Observer.OnGameReady -= GameReady;
        M_Observer.OnGameStart -= GameStart;
        M_Observer.OnGameFail -= GameFail;
        M_Observer.OnGameComplete -= GameComplete;
        M_Observer.OnGameRetry -= GameRetry;
        M_Observer.OnGameContinue -= GameContinue;
        M_Observer.OnGameNextLevel -= GameNextLevel;
    }


    private void GameCreate()
    {
        MoneyCollectedInTheGame = 0;
    }

    private void GameReady()
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
    }

    private void GameRetry()
    {
    }

    private void GameContinue()
    {
    }

    private void GameNextLevel()
    {
    }

    public void AddMoneyCollectedInTheGame(int _addedMoney)
    {
        MoneyCollectedInTheGame += _addedMoney;
        SetTotalMoney(TotalMoney + _addedMoney);
    }

    public void SetTotalMoney(int _totalMoney)
    {
        if (TotalMoney > _totalMoney)
        {
            OnMoneyDecrease?.Invoke();
        }

        TotalMoney = _totalMoney;
        RefreshInvoke();
    }


    public static void RefreshInvoke()
    {
        TotalMoneyChanged?.Invoke(TotalMoney);
    }


    public static M_Money II;

    public static M_Money I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Money").GetComponent<M_Money>();
            }

            return II;
        }
    }
}