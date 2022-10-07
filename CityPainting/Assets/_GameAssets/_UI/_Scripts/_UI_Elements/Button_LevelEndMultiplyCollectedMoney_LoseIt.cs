using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_LevelEndMultiplyCollectedMoney_LoseIt : ButtonBehaviour
{
    public override void CurrentButtonOnClick()
    {
        base.CurrentButtonOnClick();
        M_Menu.I.GoToNextScreen();
    }
}