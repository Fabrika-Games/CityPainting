using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_BackScreen : MonoBehaviour
{
    [SerializeField] private Button buttonBackScreen;

    private void Awake()
    {
        if (buttonBackScreen == null)
        {
            buttonBackScreen = gameObject.GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        buttonBackScreen.onClick.RemoveAllListeners();
        buttonBackScreen.onClick.AddListener(ButtonBackScreenOnClick);
    }

    private void OnDisable()
    {
        buttonBackScreen.onClick.RemoveAllListeners();
    }

    void ButtonBackScreenOnClick()
    {
        UIManager.I.QueuePop();
    }
}