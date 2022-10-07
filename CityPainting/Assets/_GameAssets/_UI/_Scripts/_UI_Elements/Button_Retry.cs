using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Retry : ButtonBehaviour
{
    public override void CurrentButtonOnClick()
    {
        base.CurrentButtonOnClick();
        M_Observer.OnGameRetry?.Invoke();
    }
}