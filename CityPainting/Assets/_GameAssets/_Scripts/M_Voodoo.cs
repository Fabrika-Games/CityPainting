using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Voodoo : MonoBehaviour
{
    private void Awake()
    {
        II = this;
        M_Observer.OnGameCreate += GameCreate;
        M_Observer.OnGameFail += GameFail;
        M_Observer.OnGameComplete += GameComplete;
    }

    private void OnDestroy()
    {
        M_Observer.OnGameCreate -= GameCreate;
        M_Observer.OnGameFail -= GameFail;
        M_Observer.OnGameComplete -= GameComplete;
    }

    private void GameCreate()
    {
        print("GameCreate");
        TinySauce.OnGameStarted(M_Level.LevelNumber.ToString());
    }

    private void GameFail()
    {
        print("GameFail");
        TinySauce.OnGameFinished(false, 0, M_Level.LevelNumber.ToString());
    }

    private void GameComplete()
    {
        print("GameComplete");
        TinySauce.OnGameFinished(true, 0, M_Level.LevelNumber.ToString());
    }


    public static M_Voodoo II;

    public static M_Voodoo I
    {
        get
        {
            if (II == null)
            {
                GameObject _g = GameObject.Find("M_Voodoo");
                if (_g != null)
                {
                    II = _g.GetComponent<M_Voodoo>();
                }
            }

            return II;
        }
    }
}