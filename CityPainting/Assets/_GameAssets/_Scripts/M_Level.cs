using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class M_Level : MonoBehaviour
{
    [HideInInspector] public Level CurrentLevel;

    public Level[] LevelPrefabs;

    public static Action<int, string> LevelNumberChanged;
    public static Action<int, string> FakeLevelNumberChanged;

    public static int LevelNumber
    {
        set { PlayerPrefs.SetInt("LevelNumber", value); }
        get { return PlayerPrefs.GetInt("LevelNumber", 0); }
    }

    public static int LevelNumberFake
    {
        set { PlayerPrefs.SetInt("LevelNumberFake", value); }
        get { return PlayerPrefs.GetInt("LevelNumberFake", 0); }
    }

    void Awake()
    {
        II = this;
        LevelNumber = LevelNumber;
        if (PlayerPrefs.GetInt("LevelNumberFake", 0) == 0)
        {
            LevelNumberFake = LevelNumber;
        }
        else
        {
            LevelNumberFake = LevelNumberFake;
        }

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
        StartCoroutine(GameCreate());

        IEnumerator GameCreate()
        {
            M_CanvasLoading.I.LoadingOpen();
            yield return new WaitForSeconds(1f);
            DestroyAllLevelItem();
            CurrentLevel = Instantiate(LevelPrefabs[LevelNumber % LevelPrefabs.Length]);
        }


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
        LevelNumberIncrease();
        FakeLevelNumberIncrease();
    }

    private void GameRetry()
    {
        GameCreate();
    }

    private void GameContinue()
    {
    }

    private void GameNextLevel()
    {
        GameCreate();
    }


    public void LevelNumberIncrease()
    {
        SetLevelNumber(LevelNumber + 1);
    }

    public void FakeLevelNumberIncrease()
    {
        SetFakeLevelNumber(LevelNumberFake + 1);
    }

    public void SetLevelNumber(int _levelNumber)
    {
        LevelNumber = _levelNumber;
        RefreshInvoke();
    }

    public void SetFakeLevelNumber(int _fakeLevelNumber)
    {
        LevelNumberFake = _fakeLevelNumber;
        RefreshInvoke();
    }

    public static void RefreshInvoke()
    {
        LevelNumberChanged?.Invoke(LevelNumber, "Level " + (LevelNumber + 1));
        FakeLevelNumberChanged?.Invoke(LevelNumberFake, "Level " + (LevelNumberFake + 1));
    }

    public void DestroyAllLevelItem()
    {
        if (CurrentLevel != null)
        {
            Destroy(CurrentLevel.gameObject);
        }
    }

    public static M_Level II;

    public static M_Level I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Level").GetComponent<M_Level>();
            }

            return II;
        }
    }
}