using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_GDPR : ButtonBehaviour
{
    public override void CurrentButtonOnClick()
    {
        base.CurrentButtonOnClick();
        Debug.Log("Click GDPR button");
    }
}