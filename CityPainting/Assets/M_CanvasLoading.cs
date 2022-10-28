using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class M_CanvasLoading : MonoBehaviour
{
    public RectTransform CanvasRectTransform;
    public RectTransform RadialGradient;



    void OnEnable()
    {


        M_Observer.OnGameReady += GameReady;

    }

    private void OnDisable()
    {
        M_Observer.OnGameReady -= GameReady;

    }
    private void GameReady()
    {
        LoadingClose();
    }



    public void LoadingOpen()
    {
        CanvasRectTransform.gameObject.SetActive(true);
        RadialGradient.anchoredPosition = new Vector2(-RadialGradient.sizeDelta.x * 0.5f - CanvasRectTransform.rect.width * 0.5f, RadialGradient.sizeDelta.y * 0.5f + CanvasRectTransform.rect.height * 0.5f);
        RadialGradient.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.OutSine);
    }
    public void LoadingClose()
    {
        CanvasRectTransform.gameObject.SetActive(true);
        RadialGradient.DOAnchorPos(new Vector2(-RadialGradient.sizeDelta.x * 0.5f - CanvasRectTransform.rect.width * 0.5f, RadialGradient.sizeDelta.y * 0.5f + CanvasRectTransform.rect.height * 0.5f), 1f).SetEase(Ease.InSine).OnComplete(() =>
        {
            CanvasRectTransform.gameObject.SetActive(false);
        });
    }
    private void Start()
    {

    }
    private void Awake()
    {
        II = this;
    }
    public static M_CanvasLoading II;

    public static M_CanvasLoading I
    {
        get
        {
            if (II == null)
            {
                GameObject _g = GameObject.Find("M_CanvasLoading");
                if (_g != null)
                {
                    II = _g.GetComponent<M_CanvasLoading>();
                }
            }

            return II;
        }
    }

}