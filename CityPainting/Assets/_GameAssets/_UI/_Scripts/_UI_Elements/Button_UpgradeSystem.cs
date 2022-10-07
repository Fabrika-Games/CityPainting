using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Button_UpgradeSystem : ButtonBehaviour
{
    public UpgradeType CurrentUpgradeType;
    public UpgradeButtonStatus CurrentUpgradeButtonStatus;
    public int NumberOfUpgradesForRW = 2;
    [SerializeField] private TextMeshProUGUI upgradeMoneyText;
    [SerializeField] private TextMeshProUGUI upgradeLevelText;
    [SerializeField] private Slider slider;
    [SerializeField] private Image RWImage;

    [SerializeField] private Color upgradeMoneyTextColorDisable;
    [SerializeField] private Color upgradeMoneyTextColor;

    private Vector3 beginScale;
    private Vector3 beginLocalEulerAngles;
    private int currentNumberOfUpgrades = 0;

    public override void CurrentButtonOnClick()
    {
        base.CurrentButtonOnClick();
        M_Upgrade.I.OnClickUpgradeButton(this);
    }


    public override void OnEnableVirtual()
    {
        currentNumberOfUpgrades = 0;
        beginScale = transform.localScale;
        beginLocalEulerAngles = transform.localEulerAngles;
        RefreshUI();
        M_Upgrade.OnUpgradeSuccessful += OnUpgradeSuccessful;
        M_Money.OnNoMoney += OnNoMoney;
        M_Ads.OnRewardedAvaileableChanged += RefreshUI;
    }

    public override void OnDisableVirtual()
    {
        M_Upgrade.OnUpgradeSuccessful -= OnUpgradeSuccessful;
        M_Money.OnNoMoney -= OnNoMoney;
        M_Ads.OnRewardedAvaileableChanged -= RefreshUI;
    }

    void OnUpgradeSuccessful(UpgradeType _upgradeType)
    {
        if (_upgradeType == CurrentUpgradeType)
        {
            currentNumberOfUpgrades++;
        }

        RefreshUI();
    }

    void RefreshUI()
    {
        int _targetMoney = M_Upgrade.I.GetTargetMoney(CurrentUpgradeType);
        if (_targetMoney == -1)
        {
            CurrentUpgradeButtonStatus = UpgradeButtonStatus.MaxLevel;
            upgradeMoneyText.text = "Max...";
            upgradeLevelText.text = "Max...";
            upgradeMoneyText.color = upgradeMoneyTextColor;
            upgradeMoneyText.gameObject.SetActive(true);
            RWImage.gameObject.SetActive(false);
        }
        else
        {
            if (M_Ads.I.IsRewardedAvaileable)
            {
                if (currentNumberOfUpgrades < NumberOfUpgradesForRW)
                {
                    if (_targetMoney <= M_Money.TotalMoney)
                    {
                        CurrentUpgradeButtonStatus = UpgradeButtonStatus.UpgradeableWithMoney;
                        upgradeMoneyText.color = upgradeMoneyTextColor;
                        upgradeMoneyText.gameObject.SetActive(true);
                        RWImage.gameObject.SetActive(false);
                        upgradeMoneyText.text = "$" + _targetMoney;
                        upgradeLevelText.text = M_Upgrade.I.GetLevelText(CurrentUpgradeType);
                    }
                    else
                    {
                        upgradeMoneyText.color = upgradeMoneyTextColorDisable;
                        upgradeMoneyText.text = "$" + _targetMoney;
                        upgradeLevelText.text = M_Upgrade.I.GetLevelText(CurrentUpgradeType);
                        CurrentUpgradeButtonStatus = UpgradeButtonStatus.UpgradeableWithRW;
                        upgradeMoneyText.gameObject.SetActive(false);
                        RWImage.gameObject.SetActive(true);
                    }
                }
                else
                {
                    upgradeMoneyText.color = upgradeMoneyTextColorDisable;
                    upgradeMoneyText.text = "$" + _targetMoney;
                    upgradeLevelText.text = M_Upgrade.I.GetLevelText(CurrentUpgradeType);
                    CurrentUpgradeButtonStatus = UpgradeButtonStatus.UpgradeableWithRW;
                    upgradeMoneyText.gameObject.SetActive(false);
                    RWImage.gameObject.SetActive(true);
                }
            }
            else
            {
                if (_targetMoney <= M_Money.TotalMoney)
                {
                    CurrentUpgradeButtonStatus = UpgradeButtonStatus.UpgradeableWithMoney;
                    upgradeMoneyText.color = upgradeMoneyTextColor;
                    upgradeMoneyText.gameObject.SetActive(true);
                    RWImage.gameObject.SetActive(false);
                    upgradeMoneyText.text = "$" + _targetMoney;
                    upgradeLevelText.text = M_Upgrade.I.GetLevelText(CurrentUpgradeType);
                }
                else
                {
                    CurrentUpgradeButtonStatus = UpgradeButtonStatus.NoMoney;
                    upgradeMoneyText.color = upgradeMoneyTextColorDisable;
                    upgradeMoneyText.gameObject.SetActive(true);
                    RWImage.gameObject.SetActive(false);
                    upgradeMoneyText.text = "$" + _targetMoney;
                    upgradeLevelText.text = M_Upgrade.I.GetLevelText(CurrentUpgradeType);
                }
            }
        }
    }


    private void OnNoMoney(Transform _buttonTransform)
    {
        if (_buttonTransform == transform)
        {
            transform.DOKill();
            transform.localScale = beginScale;
            transform.localEulerAngles = beginLocalEulerAngles;
            transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.5f);
            transform.DOPunchRotation(new Vector3(5, 5, 5), 0.5f);
        }
    }
}