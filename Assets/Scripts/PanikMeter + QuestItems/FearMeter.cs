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
    [SerializeField] private Sprite _midSprite;
    [SerializeField] private Sprite _fearfulSprite;
    [SerializeField] private Sprite _happySprite;

    [SerializeField] public int _winValue;
    [SerializeField, Tooltip("Must be negative.")] public int _loseValue;

    private float _happyThreshold;
    private float _fearfulThreshold;

    private float _shakeThreshold;
    private bool _isShaking;

    public static event Action OnHappinessMax;
    public static event Action OnFearMax;

    #endregion

    #region Methods

    private void Awake()
    {
        _happyThreshold = _winValue / 2f;
        _fearfulThreshold = _loseValue / 2f;

        _fearMeter = GetComponent<Slider>();
        //_fearMeter.value = 0;
        _fearLevel = GameObject.FindFirstObjectByType<FearIdentifier>();

        _shakeThreshold = _winValue * 0.9f;
    }

    private void OnEnable()
    {
        FearMeter.OnFearMax += DestroyThisSlider;
        FearMeter.OnHappinessMax += DestroyThisSlider;
    }

    private void OnDisable()
    {
        FearMeter.OnFearMax -= DestroyThisSlider;
        FearMeter.OnHappinessMax -= DestroyThisSlider;
    }

    private void Update()
    {
        //Temporär wieder entkommentiert
        _fearMeter.value = -_fearLevel.globalFearValu;
        //
        if (_fearMeter.value >= _winValue)
            OnHappinessMax?.Invoke();
        else if (_fearMeter.value >= _happyThreshold)
            _sliderHandle.sprite = _fearfulSprite;
        else if (_fearMeter.value <= _loseValue)
            OnFearMax?.Invoke();
        else if (_fearMeter.value <= _fearfulThreshold)
            _sliderHandle.sprite = _happySprite;
        else
            _sliderHandle.sprite = _midSprite;

        if (Mathf.Abs(_fearMeter.value) > _shakeThreshold && !_isShaking)
            Shake();

    }

    private void DestroyThisSlider()
    {
        gameObject.SetActive(false);// Destroy(gameObject);
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
