using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Container_LevelInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelInfoText;


    private void OnEnable()
    {
        M_Level.FakeLevelNumberChanged += FakeLevelNumberChanged;
        M_Level.RefreshInvoke();
    }


    private void OnDisable()
    {
        M_Level.FakeLevelNumberChanged -= FakeLevelNumberChanged;
    }

    private void FakeLevelNumberChanged(int levelNumber, string levelNumberText)
    {
        levelInfoText.text = levelNumberText;
    }
}