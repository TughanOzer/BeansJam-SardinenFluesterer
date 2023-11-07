using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FearMeter : MonoBehaviour
{
    #region Fields and Properties

    private Slider _fearMeter;
    private FearIdentifier _fearLevel;

    [SerializeField] private Image _sliderHandle;
    [SerializeField] private Sprite _veryHappySprite;
    [SerializeField] private Sprite _happySprite;
    [SerializeField] private Sprite _unhappySprite;
    [SerializeField] private Sprite _fearfulSprite;

    public int _loseValue;
    int sliderThreshold;

    private float _shakeThreshold;
    private bool _isShaking;

    public static event Action OnFearMax;

    #endregion

    #region Methods

    private void Awake()
    {

        _fearMeter = GetComponent<Slider>();
        _fearMeter.value = 0;
        _fearLevel = GameObject.FindFirstObjectByType<FearIdentifier>();

        _shakeThreshold = _loseValue * 0.9f;
        sliderThreshold = _loseValue / 4;
    }

    private void OnEnable()
    {
        FearMeter.OnFearMax += DestroyThisSlider;
    }

    private void OnDisable()
    {
        FearMeter.OnFearMax -= DestroyThisSlider;
    }

    private void Update()
    {
        //Temporär wieder entkommentiert
        _fearMeter.value = -_fearLevel.globalFearValu;

        if (_fearMeter.value >= _loseValue)
            OnFearMax?.Invoke();
        
        if (_fearMeter.value <= sliderThreshold)
            _sliderHandle.sprite = _veryHappySprite;
        else if (_fearMeter.value <= sliderThreshold*2)
            _sliderHandle.sprite = _happySprite;
        else if (_fearMeter.value <= sliderThreshold*3)
            _sliderHandle.sprite = _unhappySprite;
        else
            _sliderHandle.sprite = _fearfulSprite;

        if (Mathf.Abs(_fearMeter.value) > _shakeThreshold && !_isShaking)
            Shake();
        
    }

    private void DestroyThisSlider()
    {
        if(_fearMeter.value >= _loseValue)
        {
            GameObject.FindObjectOfType<WinLoseHandler>().SetMessage("Your girlfriend ran away in fear!");
        }
        Destroy(gameObject);
    }

    private void Shake()
    {
        _isShaking = true;
        transform.DOShakePosition(1, 3, 10).OnComplete(FinishedShaking);
    }

    private void FinishedShaking()
    {
        _isShaking = false;
    }

    #endregion
}
