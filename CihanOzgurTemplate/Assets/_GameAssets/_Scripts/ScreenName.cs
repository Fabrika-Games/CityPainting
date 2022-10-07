using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenName : MonoBehaviour
{
    public TextMeshProUGUI ScrenNameText;

    void Start()
    {
        ScrenNameText.text = transform.parent.name;
    }
}