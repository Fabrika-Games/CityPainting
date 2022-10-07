using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;


public class LevelEndUnlockScreen : ScreenUI
{
    public Transform GodLight;
    public TextMeshProUGUI PercentText;
    public Image UnlockImageStroke;
    public Image UnlockImageColourful;

    [SerializeField] private float fillAnimationDuration = 0.75f;
    [SerializeField] private Button_LevelEndUnlock_FillFaster Button_LevelEndUnlock_FillFaster;
    [SerializeField] private Button_LevelEndUnlock_NoThanks Button_LevelEndUnlock_NoThanks;
    [SerializeField] private Button_LevelEndUnlock_NextLevel Button_LevelEndUnlock_NextLevel;

    private float beginFillAmount = 0;
    private float endFillAmount = 0;

    public override void OnSetup()
    {
    }

    public override void OnPush(Data data)
    {
        PushFinished();
        FillAnimationPlay();
    }


    public override void OnPop()
    {
        PopFinished();
    }

    public override void OnFocus()
    {
    }

    public override void OnFocusLost()
    {
    }


    private void FillAnimationPlay()
    {
        GodLight.DOKill();
        GodLight.gameObject.SetActive(false);
        UnlockImageStroke.transform.DOKill();
        UnlockImageStroke.transform.localScale = new Vector3(1, 1, 1);
        TurnOffAllButtons();

        int _levelNumber = M_Level.LevelNumber - 1;
        int _dataIndex = 0;

        UnlockAssets[] _unlockAsset = M_Data.I.UnlockSystemData.UnlockAssets;
        int _totalUnlockLevelCount = 0;
        for (int i = 0; i < _unlockAsset.Length; i++)
        {
            _totalUnlockLevelCount += _unlockAsset[i].UnlockLevelCount;
            if (_totalUnlockLevelCount > _levelNumber)
            {
                _dataIndex = i;
                _totalUnlockLevelCount -= _unlockAsset[i].UnlockLevelCount;
                break;
            }
        }

        beginFillAmount =
            1f * (_levelNumber - _totalUnlockLevelCount) /
            (_unlockAsset[_dataIndex].UnlockLevelCount);
        endFillAmount =
            1f * (_levelNumber + 1 - _totalUnlockLevelCount) /
            (_unlockAsset[_dataIndex].UnlockLevelCount);

        UnlockImageStroke.sprite = _unlockAsset[_dataIndex].UnlockStrokeSprite;
        UnlockImageColourful.sprite = _unlockAsset[_dataIndex].UnlockColourfulSprite;

        DOTween.To(
            () => beginFillAmount,
            x => UnlockImageColourful.fillAmount = x,
            endFillAmount,
            fillAnimationDuration
        ).OnUpdate(() =>
        {
            PercentText.text = string.Format("{0}% completed!", (int) (UnlockImageColourful.fillAmount * 100));
        }).OnComplete(() =>
        {
            if (endFillAmount >= 0.95f)
            {
                UnlockItem();
            }
            else
            {
                RefreshAllButtons();
            }
        });
    }

    void UnlockItem()
    {
        GodLight.gameObject.SetActive(true);
        GodLight.localScale = new Vector3();
        GodLight.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.OutFlash);
        GodLight.DOLocalRotate(new Vector3(0, 0, 360), 6, RotateMode.FastBeyond360).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        UnlockImageStroke.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 1).SetEase(Ease.OutElastic)
            .OnComplete(RefreshAllButtons);
    }

    void TurnOffAllButtons()
    {
        Button_LevelEndUnlock_FillFaster.gameObject.SetActive(false);
        Button_LevelEndUnlock_NoThanks.gameObject.SetActive(false);
        Button_LevelEndUnlock_NextLevel.gameObject.SetActive(false);
    }

    void RefreshAllButtons()
    {
        if (M_Ads.I.IsRewardedAvaileable &&
            1 - (endFillAmount - beginFillAmount) > UnlockImageColourful.fillAmount)
        {
            Button_LevelEndUnlock_FillFaster.gameObject.SetActive(true);
            Button_LevelEndUnlock_NoThanks.gameObject.SetActive(true);
            Button_LevelEndUnlock_NextLevel.gameObject.SetActive(false);
        }
        else
        {
            Button_LevelEndUnlock_FillFaster.gameObject.SetActive(false);
            Button_LevelEndUnlock_NoThanks.gameObject.SetActive(false);
            Button_LevelEndUnlock_NextLevel.gameObject.SetActive(true);
        }
    }

    public void FillFasterRewardedVideoWatched()
    {
        M_Level.I.LevelNumberIncrease();
        FillAnimationPlay();
    }
}