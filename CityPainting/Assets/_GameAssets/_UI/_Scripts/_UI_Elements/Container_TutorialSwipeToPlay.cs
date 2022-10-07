using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Container_TutorialSwipeToPlay : MonoBehaviour, IBeginDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        M_Observer.OnGameStart?.Invoke();
    }
}