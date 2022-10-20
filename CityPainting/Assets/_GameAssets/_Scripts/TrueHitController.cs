using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrueHitController : MonoBehaviour
{
    public void Setup(Cube _currentCube, Vector3 _hitPoint)
    {
        _currentCube.CurrentTrueHitController = this;
        transform.SetParent(_currentCube.transform);
        transform.position = _hitPoint;
        transform.localScale = new Vector3(0, 0, 0);
        transform.DOScale(new Vector3(20, 20, 20), 1.0f).SetEase(Ease.InOutExpo);
    }
}