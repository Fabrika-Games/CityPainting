using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Finger3D : MonoBehaviour
{

    public Transform Container;
    public Transform HandContainer;
    private Transform cameraPosition;
    private Renderer[] renderers;
    private List<Material> materials = new List<Material>();

    public static Finger3D I;
    private float alpha;
    void Awake()
    {
        I = this;
        cameraPosition = M_Camera.I.CameraPerspective.transform;
        renderers = Container.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
            {
                Material _m = renderers[i].sharedMaterials[j];
                if (materials.Contains(_m) == false)
                {
                    materials.Add(_m);
                }
            }
        }
        Close();
    }
    private void OnEnable()
    {
        M_Observer.OnGameCreate += GameCreate;
    }
    private void OnDisable()
    {
        M_Observer.OnGameCreate -= GameCreate;
    }
    private void GameCreate()
    {
        HandContainer.localPosition = new Vector3(1000, 1000, 1000);

    }
    public void Open(RaycastHit raycastHit)
    {
        if (raycastHit.collider != null)
        {
            Container.transform.position = cameraPosition.transform.position;
            Container.transform.forward = (raycastHit.point - cameraPosition.transform.position).normalized;
            Container.transform.position += Container.transform.forward * 10;
            HandContainer.DOKill();
            HandContainer.localPosition = new Vector3(-0.25f, 0.25f, -0.25f);
            HandContainer.localScale = Vector3.zero;
            HandContainer.DOScale(Vector3.one, 0.125f).SetEase(Ease.OutBack);
            HandContainer.DOLocalMove(Vector3.zero, 0.25f).SetDelay(0.125f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                HandContainer.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutExpo);
                // HandContainer.DOLocalMove(new Vector3(-0.5f, 0.5f, -0.5f), 0.125f).SetEase(Ease.InOutExpo);
            });
            // DOTween.To((xx) =>
            // {
            //     alpha = xx;
            //     for (int i = 0; i < renderers.Length; i++)
            //     {
            //     }
            // }, alpha, 1, 0.5f);
        }
    }

    public void Close()
    {
        // DOTween.To((xx) =>
        // {
        //     alpha = xx;
        // }, alpha, 0, 0.5f);
    }

}