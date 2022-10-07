using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_StartWithPlusXXItems : ButtonBehaviour
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
        print("StartWithPlusXXItems isWatched:" + isWatched);
    }
}