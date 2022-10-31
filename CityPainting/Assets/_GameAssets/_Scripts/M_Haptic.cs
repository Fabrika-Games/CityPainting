using System;
using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;

public class M_Haptic : MonoBehaviour
{
    public HapticClip TrueHitHapticClip;

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
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }

    public void ConfetiHit()
    {
        if (!IsHapticActive)
        {
            return;
        }


    }


    public void ImpactFeedbackLight()
    {
        if (!IsHapticActive)
        {
            return;
        }


    }
    public void FalseHit()
    {
        if (!IsHapticActive)
        {
            return;
        }
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);

    }

    public void TrueHit()
    {
        if (!IsHapticActive)
        {
            return;
        }
        StartCoroutine(TrueHit());

        IEnumerator TrueHit()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
            yield return new WaitForSeconds(0.225f);
            HapticController.Play(TrueHitHapticClip);

        }

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