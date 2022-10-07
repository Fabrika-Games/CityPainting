using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RDG;

public class M_Haptic : MonoBehaviour
{

    public bool IsHapticActive
    {
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt("IsHapticActive", 1);
            }
            else
            {
                PlayerPrefs.SetInt("IsHapticActive", 0);
            }
        }
        get
        {
#if UNITY_ANDROID
            if (PlayerPrefs.GetInt("IsHapticActive", 0) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
#else
            if (PlayerPrefs.GetInt("IsHapticActive", 1) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
#endif
        }
    }

    private void Awake()
    {
        II = this;
    }

  
    public void HapticToogle()
    {
        IsHapticActive = !IsHapticActive;
    }

   

    public void ButtonClick()
    {
        if (!IsHapticActive)
        {
            return;
        }

        TapticPlugin.TapticManager.Notification(TapticPlugin.NotificationFeedback.Success);
#if UNITY_ANDROID
            Vibration.Vibrate(100);
#endif
    }

    public void ConfetiHit()
    {
        if (!IsHapticActive)
        {
            return;
        }

        TapticPlugin.TapticManager.Notification(TapticPlugin.NotificationFeedback.Success);
#if UNITY_ANDROID
            Vibration.Vibrate(50);
#endif
    }


    public void ImpactFeedbackLight()
    {
        if (!IsHapticActive)
        {
            return;
        }

        TapticPlugin.TapticManager.Impact(TapticPlugin.ImpactFeedback.Light);
#if UNITY_ANDROID
        Vibration.Vibrate(25);
#endif
    }


    public static M_Haptic II;

    public static M_Haptic I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Haptic").GetComponent<M_Haptic>();
            }

            return II;
        }
    }
}