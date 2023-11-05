using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseHandler : MonoBehaviour
{
    #region Fields and Properties

    [SerializeField] private Image _winBanner;
    [SerializeField] private Image _loseBanner;

    private bool _endStateReached;

    #endregion

    #region Methods

    private void Awake()
    {
        _winBanner.DOFade(0, 0.1f);
        _loseBanner.DOFade(0, 0.1f);
    }

    private void OnEnable()
    {
        DeathTrap.OnDeathTrapTriggered += FadeInLoseImage;
        FearMeter.OnHappinessMax += FadeInWinImage;
        FearMeter.OnFearMax += FadeInLoseImage;
    }

    private void OnDisable()
    {
        DeathTrap.OnDeathTrapTriggered -= FadeInLoseImage;
        FearMeter.OnHappinessMax -= FadeInWinImage;
        FearMeter.OnFearMax -= FadeInLoseImage;
    }

    private void Update()
    {
        if (_endStateReached && Input.anyKeyDown)
        {
            PauseControl.ResumeGame();
            LoadHelper.LoadSceneWithLoadingScreen(SceneName.MainMenu);
        }
    }

    private void FadeInLoseImage()
    {
        _loseBanner.DOFade(1, 3f).OnComplete(EndStateReached);
    }

    private void FadeInWinImage()
    {
        _winBanner.DOFade(1, 3f).OnComplete(EndStateReached);
    }

    private void EndStateReached()
    {
        PauseControl.PauseGame();
        _endStateReached = true;
    }

    #endregion
}
