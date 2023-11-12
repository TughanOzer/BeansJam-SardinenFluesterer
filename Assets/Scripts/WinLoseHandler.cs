using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinLoseHandler : MonoBehaviour
{
    #region Fields and Properties

    [SerializeField] private Image _winBanner;
    [SerializeField] private Image _loseBanner;
    [SerializeField] GameObject message;

    private bool _endStateReached;

    #endregion

    #region Methods

    private void Awake()
    {
        _winBanner.DOFade(0, 0);
        _loseBanner.DOFade(0, 0);
        message.SetActive(false);
    }

    private void OnEnable()
    {
        DeathTrap.OnDeathTrapTriggered += FadeInLoseImage;
        FearMeter.OnFearMax += FadeInLoseImage;
        ExorciseObject.OnAllObjectsFound += FadeInWinImage;
    }

    private void OnDisable()
    {
        DeathTrap.OnDeathTrapTriggered -= FadeInLoseImage;
        FearMeter.OnFearMax -= FadeInLoseImage;
        ExorciseObject.OnAllObjectsFound -= FadeInWinImage;
    }

    private void Update()
    {
        if (_endStateReached && Input.anyKeyDown)
        {
            PauseControl.ResumeGame();
            LoadHelper.LoadSceneWithLoadingScreen(SceneName.MainMenu);
        }
    }
    public void SetMessage(string s) {
        message.GetComponent<TMP_Text>().text =  s;
    }
    private void FadeInLoseImage()
    {
        _loseBanner.DOFade(1, 3f).OnComplete(EndStateReached);
        message.SetActive(true);
    }

    //auf public gesetzt
    public void FadeInWinImage()
    {
        _winBanner.DOFade(1, 3f).OnComplete(EndStateReached);
        message.SetActive(true);
    }

    private void EndStateReached()
    {
        PauseControl.PauseGame();
        _endStateReached = true;
    }

    #endregion
}
