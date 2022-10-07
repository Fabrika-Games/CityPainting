using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Continue : ButtonBehaviour
{
    public override void CurrentButtonOnClick()
    {
        base.CurrentButtonOnClick();
        if (M_Ads.I.IsRewardedAvaileable)
        {
            M_Ads.I.ShowRewardedVideo(RewardedVideoCallBack);
        }
    }

    void RewardedVideoCallBack(bool isWatched)
    {
        if (isWatched)
        {
            M_Observer.OnGameContinue?.Invoke();
        }
        print("Continue isWatched:" + isWatched);
    }
}