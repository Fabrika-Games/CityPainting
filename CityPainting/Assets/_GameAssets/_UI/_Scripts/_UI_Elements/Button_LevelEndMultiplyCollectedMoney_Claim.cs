using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_LevelEndMultiplyCollectedMoney_Claim : ButtonBehaviour
{
    [SerializeField] private TextMeshProUGUI claimText;

    void Start()
    {
        LevelEndMultiplyCollectedMoneyScreen _screen =
            UIManager.I.GetScreen<LevelEndMultiplyCollectedMoneyScreen>(ScreenIds.LevelEndMultiplyCollectedMoney);
        if (_screen != null)
        {
            _screen.ClaimText = claimText;
        }
    }

    public override void CurrentButtonOnClick()
    {
        base.CurrentButtonOnClick();
        M_Ads.I.ShowRewardedVideo(RewardedVideoCallBack);
    }

    private void RewardedVideoCallBack(bool _watched)
    {
        if (_watched)
        {
            UIManager.I.GetScreen<LevelEndMultiplyCollectedMoneyScreen>(ScreenIds.LevelEndMultiplyCollectedMoney)
                .ClaimRewardedVideoWatched();
        }
    }
}