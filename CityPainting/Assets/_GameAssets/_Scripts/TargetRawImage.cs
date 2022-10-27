using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TargetRawImage : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Image trueImage;
    [SerializeField] private Image falseImage;
    private Vector3 beginScale;
    private RectTransform rectTransform;
    private Vector3 beginAnchorPosition;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        beginScale = transform.localScale;
        beginAnchorPosition = rectTransform.anchoredPosition;
        trueImage.transform.localScale = Vector3.zero;
        falseImage.transform.localScale = Vector3.zero;

    }
    void OnEnable()
    {
        rawImage.texture = M_TargetCamera.I.CurrentCamera.targetTexture;
        M_Observer.OnTrueHitAnimationStart += TrueHitAnimationStart;
        M_Observer.OnTrueHitAnimationComplete += TrueHitAnimationComplete;
        M_Observer.OnFalseHitAnimation += FalseHitAnimation;
    }

    private void OnDisable()
    {
        M_Observer.OnTrueHitAnimationStart -= TrueHitAnimationStart;
        M_Observer.OnTrueHitAnimationComplete -= TrueHitAnimationComplete;
        M_Observer.OnFalseHitAnimation -= FalseHitAnimation;
    }
    private void FalseHitAnimation(Cube _cube)
    {
        falseImage.transform.localScale = Vector3.zero;
        falseImage.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            falseImage.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InExpo).SetDelay(0.25f);
        });
    }
    private void TrueHitAnimationComplete()
    {
        rectTransform.DOAnchorPos3D(beginAnchorPosition, 0.25f).SetEase(Ease.OutExpo);

    }
    private void TrueHitAnimationStart()
    {
        trueImage.transform.localScale = Vector3.zero;
        trueImage.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            trueImage.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InExpo).SetDelay(0.25f);
        });
        transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.25f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.InExpo).SetDelay(0.25f).OnComplete(() =>
            {
                rectTransform.DOAnchorPos3D(beginAnchorPosition + new Vector3(0, rectTransform.sizeDelta.y), 0.25f).SetEase(Ease.OutExpo);
            });
        });
    }

}