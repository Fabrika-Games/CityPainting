using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_RewardedCancelled : ButtonBehaviour
{
    public override void CurrentButtonOnClick()
    {
        base.CurrentButtonOnClick();
        M_Ads.OnRewardedReturn?.Invoke(false);
    }
}