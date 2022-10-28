using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Container_TrueHitOverCubeCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    private Vector3 beginLocalPosition;
    private void Awake()
    {
        beginLocalPosition = transform.localPosition;
    }
    private void OnEnable()
    {
        Text.text = M_Level.I.CurrentLevel.TrueHitCount + "/" + M_Level.I.CurrentLevel.CubeCount;
        M_Observer.OnTrueHitAnimationStart += TrueHitAnimationStart;
        transform.localPosition = beginLocalPosition + new Vector3(300, 0);
        transform.DOKill();
        transform.DOLocalMove(beginLocalPosition, 0.5f).SetEase(Ease.InOutExpo);
    }
    private void OnDisable()
    {
        M_Observer.OnTrueHitAnimationStart -= TrueHitAnimationStart;
    }
    private void TrueHitAnimationStart()
    {
        Text.text = M_Level.I.CurrentLevel.TrueHitCount + "/" + M_Level.I.CurrentLevel.CubeCount;
    }
}