using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Upgrade : MonoBehaviour
{
    public static Action<UpgradeType> OnUpgradeSuccessful;
    public int[] MoneyList_Speed;
    public int[] MoneyList_Power;
    public int[] MoneyList_Asdf;

    public float[] ValueList_Speed;
    public float[] ValueList_Power;
    public float[] ValueList_Asdf;


    private Button_UpgradeSystem currentButtonUpgradeSystemForRW;

    private void Awake()
    {
        II = this;
    }


    public void OnClickUpgradeButton(Button_UpgradeSystem buttonUpgradeSystem)
    {
        int _totalMoney = M_Money.TotalMoney;
        if (buttonUpgradeSystem.CurrentUpgradeButtonStatus == UpgradeButtonStatus.NoMoney)
        {
            M_Money.OnNoMoney?.Invoke(buttonUpgradeSystem.transform);
        }
        else if (buttonUpgradeSystem.CurrentUpgradeButtonStatus == UpgradeButtonStatus.UpgradeableWithRW)
        {
            currentButtonUpgradeSystemForRW = buttonUpgradeSystem;
            M_Ads.I.ShowRewardedVideo(UpgradeRewardedCallBack);
        }
        else if (buttonUpgradeSystem.CurrentUpgradeButtonStatus == UpgradeButtonStatus.UpgradeableWithMoney)
        {
            int _targetMoney = 0;
            _targetMoney = GetTargetMoney(buttonUpgradeSystem.CurrentUpgradeType);
            LevelIncrease(buttonUpgradeSystem.CurrentUpgradeType);
            M_Money.I.SetTotalMoney(_totalMoney - _targetMoney);
            OnUpgradeSuccessful?.Invoke(buttonUpgradeSystem.CurrentUpgradeType);
        }
    }

    private void UpgradeRewardedCallBack(bool _watched)
    {
        if (_watched)
        {
            LevelIncrease(currentButtonUpgradeSystemForRW.CurrentUpgradeType);
            OnUpgradeSuccessful?.Invoke(currentButtonUpgradeSystemForRW.CurrentUpgradeType);
        }
    }

    public int GetLevel(UpgradeType type)
    {
        return PlayerPrefs.GetInt(type.ToString(), 0);
    }

    public string GetLevelText(UpgradeType type)
    {
        return "Lvl." + (PlayerPrefs.GetInt(type.ToString(), 0) + 1);
    }


    public void LevelIncrease(UpgradeType type)
    {
        PlayerPrefs.SetInt(type.ToString(), PlayerPrefs.GetInt(type.ToString(), 0) + 1);
    }

    public int GetTargetMoney(UpgradeType type)
    {
        int _levelNum = 0;
        _levelNum = GetLevel(type);
        if (MoneyList_Speed.Length > _levelNum + 1)
        {
            return MoneyList_Speed[GetLevel(type)];
        }

        return -1;
    }

    public float GetValue(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Speed:
                return ValueList_Speed[GetLevel(type)];
            case UpgradeType.Power:
                return ValueList_Power[GetLevel(type)];
            case UpgradeType.Asdf:
                return ValueList_Asdf[GetLevel(type)];
        }

        return 0;
    }

    public static M_Upgrade II;

    public static M_Upgrade I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Upgrade").GetComponent<M_Upgrade>();
            }

            return II;
        }
    }
}


public enum UpgradeType
{
    Speed,
    Power,
    Asdf
}

public enum UpgradeButtonStatus
{
    UpgradeableWithMoney,
    UpgradeableWithRW,
    NoMoney,
    MaxLevel
}