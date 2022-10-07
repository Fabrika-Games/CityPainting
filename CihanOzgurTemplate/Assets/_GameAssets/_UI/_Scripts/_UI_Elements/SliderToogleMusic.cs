using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderToogleMusic : SliderToogle
{
    private void OnEnable()
    {
        slider.value = M_Audio.I.IsMusicActive == true ? 1 : 0;
    }

    public override void SliderValueChanged(float _sliderVal)
    {
        base.SliderValueChanged(_sliderVal);

        M_Audio.I.IsMusicActive = _sliderVal < 0.5f ? false : true;
        OnEnable();
    }
}