using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_Ads : MonoBehaviour
{
    public static Action<bool> OnRewardedReturn;
    public static Action OnRewardedAvaileableChanged;
    public GameObject RewardedAdsContainer;
    public bool IsRewardedAvaileable = true;

    public int RWreloadedTime = 5;

    private void Awake()
    {
        IsRewardedAvaileable = false;
        OnRewardedAvaileableChanged?.Invoke();
    }

    private void Start()
    {
        StartCoroutine(Start());

        IEnumerator Start()
        {
            yield return new WaitForSeconds(RWreloadedTime);
            IsRewardedAvaileable = true;
            OnRewardedAvaileableChanged?.Invoke();
        }
    }

    public void ShowRewardedVideo(Action<bool> _rewardedOnCallBack)
    {
        if (IsRewardedAvaileable == false)
        {
            return;
        }

        OnRewardedReturn = _rewardedOnCallBack;
        OnRewardedReturn += Rewarded_OnCallBack;
        RewardedAdsContainer.SetActive(true);
        Time.timeScale = 0;
    }


    public void Close()
    {
        OnRewardedReturn -= Rewarded_OnCallBack;
        RewardedAdsContainer.SetActive(false);
        Time.timeScale = 1;
    }

    public void Rewarded_OnCallBack(bool isWatched)
    {
        if (isWatched)
        {
            IsRewardedAvaileable = false;
            OnRewardedAvaileableChanged?.Invoke();
            StartCoroutine(Rewarded_OnCallBack());

            IEnumerator Rewarded_OnCallBack()
            {
                yield return new WaitForSeconds(RWreloadedTime);
                IsRewardedAvaileable = true;
                OnRewardedAvaileableChanged?.Invoke();
            }
        }


        Close();
    }

    public static M_Ads II;

    public static M_Ads I
    {
        get
        {
            if (II == null)
            {
                GameObject _g = GameObject.Find("M_Ads");
                if (_g != null)
                {
                    II = _g.GetComponent<M_Ads>();
                }
            }

            return II;
        }
    }
}