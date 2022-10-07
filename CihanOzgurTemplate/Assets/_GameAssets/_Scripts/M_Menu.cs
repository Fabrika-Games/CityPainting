using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Menu : MonoBehaviour
{
    private int getScreenIdIndex = 0;

    public ScreenUI.Id[] GameCompleteFlow = new ScreenUI.Id[]
    {
        ScreenIds.LevelEndMultiplyCollectedMoney,
        ScreenIds.LevelEndUnlock,
        ScreenIds.MainMenu
    };


    private void Awake()
    {
        II = this;
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
    }

    private void GameReady()
    {
        UIManager.I.AllScreensPop();
        UIManager.I.QueuePush(ScreenIds.MainMenu);
    }

    private void GameStart()
    {
        UIManager.I.AllScreensPop();
        UIManager.I.QueuePush(ScreenIds.Game);
    }

    private void GameFail()
    {
        UIManager.I.AllScreensPop();
        UIManager.I.QueuePush(ScreenIds.FailMenu);
    }

    private void GameComplete()
    {
        UIManager.I.AllScreensPop();
        UIManager.I.QueuePush(GetNextScreenId(true));
        // if (M_Config.I.IsActiveSystem_MultiplyCollectedMoney)
        // {
        //     UIManager.I.AllScreensPop();
        //     UIManager.I.QueuePush(ScreenIds.MultiplyCollectedMoney);
        // }
        // else
        // {
        //     UIManager.I.AllScreensPop();
        //     UIManager.I.QueuePush(ScreenIds.CompleteMenu);
        // }
    }

    private void GameRetry()
    {
        UIManager.I.AllScreensPop();
        UIManager.I.QueuePush(ScreenIds.MainMenu);
    }

    private void GameContinue()
    {
        UIManager.I.AllScreensPop();
        UIManager.I.QueuePush(ScreenIds.Game);
    }

    private void GameNextLevel()
    {
        UIManager.I.AllScreensPop();
        UIManager.I.QueuePush(ScreenIds.MainMenu);
    }

    public void GoToNextScreen()
    {
        UIManager.I.AllScreensPop();
        UIManager.I.QueuePush(GetNextScreenId());
    }

    public ScreenUI.Id GetNextScreenId(bool isFirst = false)
    {
        if (isFirst)
        {
            getScreenIdIndex = 0;
        }

        ScreenUI.Id _id = GameCompleteFlow[getScreenIdIndex];
        if (_id == ScreenIds.MainMenu)
        {
            M_Observer.OnGameCreate?.Invoke();
            return null;
        }
        else
        {
            getScreenIdIndex++;
            return _id;
        }
    }

    public static M_Menu II;

    public static M_Menu I
    {
        get
        {
            if (II == null)
            {
                GameObject _g = GameObject.Find("M_Menu");
                if (_g != null)
                {
                    II = _g.GetComponent<M_Menu>();
                }
            }

            return II;
        }
    }
}