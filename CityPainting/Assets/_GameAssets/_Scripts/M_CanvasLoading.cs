using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class M_CanvasLoading : MonoBehaviour
{
    public RectTransform CanvasRectTransform;

    private List<Vector3> beginLocalPositions = new List<Vector3>();

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
        for (int i = 0; i < CanvasRectTransform.childCount; i++)
        {
            Transform _t = CanvasRectTransform.GetChild(i);
            Vector3 _beginLocalPosition = beginLocalPositions[i];
            _t.localPosition = _beginLocalPosition + _beginLocalPosition.normalized * 1500;
            _t.DOLocalMove(_beginLocalPosition, 1).SetEase(Ease.OutSine);
        }
    }
    public void LoadingClose()
    {
        CanvasRectTransform.gameObject.SetActive(true);
        for (int i = 0; i < CanvasRectTransform.childCount; i++)
        {
            Transform _t = CanvasRectTransform.GetChild(i);
            Vector3 _beginLocalPosition = beginLocalPositions[i];
            if (i != CanvasRectTransform.childCount - 1)
            {
                _t.DOLocalMove(_beginLocalPosition + _beginLocalPosition.normalized * 1500, 1).SetEase(Ease.OutSine);

            }
            else
            {
                _t.DOLocalMove(_beginLocalPosition + _beginLocalPosition.normalized * 1500, 1).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    CanvasRectTransform.gameObject.SetActive(false);

                });

            }
        }
    }
    private void Start()
    {

    }
    private void Awake()
    {
        II = this;
        for (int i = 0; i < CanvasRectTransform.childCount; i++)
        {
            beginLocalPositions.Add(CanvasRectTransform.GetChild(i).localPosition);
        }
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