using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(EventTrigger))]
public class SliderToogle : MonoBehaviour, IPointerClickHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler
{
    public Slider slider;
    [SerializeField] private EventTrigger eventTrigger;

    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        if (eventTrigger == null)
        {
            eventTrigger = GetComponent<EventTrigger>();
        }
    }

    private bool isClick = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClick)
        {
            return;
        }

        if (slider.value < 0.5f)
        {
            slider.value = 1;
        }
        else
        {
            slider.value = 0;
        }

        SliderValueChanged(slider.value);
    }

    private float dragX = 0;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isClick = false;
        dragX = eventData.position.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragX < eventData.position.x)
        {
            slider.value = 1;
        }
        else
        {
            slider.value = 0;
        }

        SliderValueChanged(slider.value);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isClick = true;
    }

    public virtual void SliderValueChanged(float _sliderValue)
    {
    }
}