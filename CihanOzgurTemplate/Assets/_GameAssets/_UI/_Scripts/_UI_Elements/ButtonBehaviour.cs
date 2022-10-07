using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    public Button currentButton;

    private void Awake()
    {
        if (currentButton == null)
        {
            currentButton = gameObject.GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        currentButton.onClick.RemoveAllListeners();
        currentButton.onClick.AddListener(CurrentButtonOnClick);
        OnEnableVirtual();
    }

    public virtual void OnEnableVirtual()
    {
    }

    public virtual void OnDisableVirtual()
    {
    }

    private void OnDisable()
    {
        currentButton.onClick.RemoveAllListeners();
        OnDisableVirtual();
    }


    public virtual void CurrentButtonOnClick()
    {
        M_Haptic.I.ButtonClick();
    }
}