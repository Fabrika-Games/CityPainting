using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;


public class LevelEndMultiplyCollectedMoneyScreen : ScreenUI
{
    public TextMeshProUGUI MoneyCollectedInTheGameInfoText;
    [SerializeField] private Transform Container_MoneyInfo_Transform;
    [SerializeField] private RectTransform collectedMoneyTarget;
    [SerializeField] private RectTransform scatterMoneyBegin;
    [SerializeField] private RectTransform collectedMoneyAnimationPrefab;
    [SerializeField] private Animator arrowAnimator;
    [SerializeField] private Transform buttonContainer;
    [HideInInspector] public TextMeshProUGUI ClaimText;
    private int claimMultiplier = 1;

    public override void OnSetup()
    {
        // Run one-time setup operations here.
    }

    public override void OnPush(Data data)
    {
        // Be sure to call PushFinished to signal the end of the push.
        PushFinished();

        if (M_Money.I.MoneyCollectedInTheGame == 0)
        {
            M_Menu.I.GoToNextScreen();
            return;
        }

        arrowAnimator.speed = 1;
        MoneyCollectedInTheGameInfoText.text = M_Money.I.MoneyCollectedInTheGame.ToString();
        buttonContainer.gameObject.SetActive(true);
    }

    public override void OnPop()
    {
        // Be sure to call PopFinished to signal the end of the pop.
        PopFinished();
    }

    public override void OnFocus()
    {
    }

    public override void OnFocusLost()
    {
    }

    public void ClaimRewardedVideoWatched()
    {
        arrowAnimator.speed = 0;
        buttonContainer.gameObject.SetActive(false);
        Rect _canvasRect = UIManager.I.rootCanvas.pixelRect;
        List<RectTransform> _collectedMoneys = new List<RectTransform>();
        int _startVal = M_Money.I.MoneyCollectedInTheGame;
        int _endVal = M_Money.I.MoneyCollectedInTheGame * claimMultiplier;
        DOTween.To((xx) => { MoneyCollectedInTheGameInfoText.text = ((int) xx).ToString(); },
            _startVal,
            _endVal,
            0.75f).SetEase(Ease.OutFlash).OnUpdate(() =>
        {
            if (_endVal - _startVal > _collectedMoneys.Count)
            {
                RectTransform _m = Instantiate(collectedMoneyAnimationPrefab, collectedMoneyTarget);
                _m.position = scatterMoneyBegin.position;
                _m.localScale = new Vector3(0, 0, 0);
                _m.DOScale(new Vector3(1, 1, 1), 0.25f).SetEase(Ease.OutElastic);
                Vector2 _rV2 = new Vector2(Random.Range(-_canvasRect.width, _canvasRect.width) * 0.5f,
                                   Random.Range(-_canvasRect.height, _canvasRect.height) * 0.5f) +
                               _m.anchoredPosition;
                _m.DOJumpAnchorPos(_rV2, 600, 1, 0.5f);
                _collectedMoneys.Add(_m);
            }
        }).OnComplete(() =>
        {
            for (int i = 0; i < _collectedMoneys.Count; i++)
            {
                RectTransform _m = _collectedMoneys[i];
                float _delay = Random.Range(0, 1f) + 0.5f;
                _m.DOLocalJump(new Vector2(), 1, 1, 0.5f).SetEase(Ease.OutFlash).SetDelay(_delay);
                _m.DOScale(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.OutFlash).SetDelay(_delay).OnComplete(() =>
                {
                    Destroy(_m.gameObject);
                });
            }

            DOTween.To((xx) => { M_Money.I.SetTotalMoney((int) xx); },
                M_Money.TotalMoney + M_Money.I.MoneyCollectedInTheGame,
                M_Money.TotalMoney + M_Money.I.MoneyCollectedInTheGame * (claimMultiplier - 1),
                1.5f).SetEase(Ease.OutFlash).SetDelay(0.5f).OnComplete(() =>
            {
                Container_MoneyInfo_Transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
                Container_MoneyInfo_Transform.DOPunchRotation(new Vector3(10, 10, 10), 0.5f).OnComplete(() =>
                {
                    Container_MoneyInfo_Transform.localEulerAngles = new Vector3();
                    M_Menu.I.GoToNextScreen();
                });
            });
        });
    }


    private void LateUpdate()
    {
        ArrowRefresh();
    }


    public void ArrowRefresh()
    {
        float posX = arrowAnimator.transform.localPosition.x;
        if ((posX > -350 && posX <= -250) || (posX > 250 && posX <= 350))
        {
            ClaimText.text = "Claim 2x";
            claimMultiplier = 2;
        }
        else if ((posX > -250 && posX <= -150) || (posX > 150 && posX <= 250))
        {
            ClaimText.text = "Claim 3x";
            claimMultiplier = 3;
        }
        else if ((posX > -150 && posX <= -50) || (posX > 50 && posX <= 150))
        {
            ClaimText.text = "Claim 4x";
            claimMultiplier = 4;
        }
        else if (posX > -50 && posX <= 50)
        {
            ClaimText.text = "Claim 5x";
            claimMultiplier = 5;
        }
    }
}

