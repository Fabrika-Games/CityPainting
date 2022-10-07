using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Container_MoneyInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MoneyInfoText;

    private Vector3 beginScale;
    private Vector3 beginLocalEulerAngles;

    private void OnEnable()
    {
        M_Money.TotalMoneyChanged += TotalMoneyChanged;
        M_Money.RefreshInvoke();
        beginScale = transform.localScale;
        beginLocalEulerAngles = transform.localEulerAngles;
        M_Money.OnNoMoney += OnNoMoney;
        M_Money.OnMoneyDecrease += OnMoneyDecrease;
    }

    private void OnDisable()
    {
        M_Money.TotalMoneyChanged -= TotalMoneyChanged;
        M_Money.OnNoMoney -= OnNoMoney;
        M_Money.OnMoneyDecrease -= OnMoneyDecrease;
    }

    private void OnMoneyDecrease()
    {
        transform.DOKill();
        transform.localScale = beginScale;
        transform.DOScale(beginScale * 1.25f, 0.2f).OnComplete(() => { transform.DOScale(beginScale, 0.5f); });
    }

    private void OnNoMoney(Transform obj)
    {
        transform.DOKill();
        transform.localScale = beginScale;
        transform.localEulerAngles = beginLocalEulerAngles;
        transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.5f);
        transform.DOPunchRotation(new Vector3(5, 5, 5), 0.5f);
    }

    private void TotalMoneyChanged(int _totalMoney)
    {
        MoneyInfoText.text = _totalMoney.ToString();
    }
}