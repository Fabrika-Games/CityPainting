using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderToogleVibration : SliderToogle
{
    private void OnEnable()
    {
        slider.value = M_Haptic.I.IsHapticActive == true ? 1 : 0;
    }

    public override void SliderValueChanged(float _sliderVal)
    {
        base.SliderValueChanged(_sliderVal);

        M_Haptic.I.IsHapticActive = _sliderVal < 0.5f ? false : true;
        OnEnable();
    }
}