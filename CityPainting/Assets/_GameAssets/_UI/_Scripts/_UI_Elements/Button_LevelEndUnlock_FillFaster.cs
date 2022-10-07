using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_LevelEndUnlock_FillFaster : ButtonBehaviour
{
    public override void CurrentButtonOnClick()
    {
        base.CurrentButtonOnClick();
        M_Ads.I.ShowRewardedVideo(RewardedVideoCallBack);
    }

    private void RewardedVideoCallBack(bool _watched)
    {
        if (_watched)
        {
            UIManager.I.GetScreen<LevelEndUnlockScreen>(ScreenIds.LevelEndUnlock)
                .FillFasterRewardedVideoWatched();
        }
    }
}