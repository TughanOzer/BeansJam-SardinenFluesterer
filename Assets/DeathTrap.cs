using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class DeathTrap : MonoBehaviour
{
    #region Fields and Properties

    AudioSource sound;
    [SerializeField] AudioClip arming;
    [SerializeField] AudioClip removed;
    [SerializeField] AudioClip triggered;

    [SerializeField] private float _disarmingTime = 1;
    [SerializeField] private float _armingTime = 5;
    [SerializeField] private bool _isCleaver;

    private float _timer;

    [SerializeField] private Image _timerImage;
    [SerializeField] private List<Sprite> _timerSprites = new();
    [SerializeField] GameObject playerPrompt;
    [SerializeField] Slider unarmingTimeUI;
    [SerializeField] TextMeshProUGUI timeRemaining;
    private float _disarmingIncrements;
    private float _armingIncrements;
    private int _currentSpriteIndex = 0;

    private bool _isBeingArmed;
    private bool _isBeingDisarmed;
    private bool _isArmed;
    bool playerInRange = false;

    #endregion

    #region Events and Event Methods

    public static event Action OnDeathTrapTriggered;

    #endregion

    #region Methods

    private void Start()
    {
        sound = GetComponent<AudioSource>(); 
        var renderer = GetComponent<SpriteRenderer>();
        renderer.DOFade(0, 0.01f);
        _timerImage.DOFade(0, 0.01f);

        _disarmingIncrements = _disarmingTime / 6;
        _armingIncrements = _armingTime / 6;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ghost ghost) && !_isArmed && !_isBeingArmed && !_isBeingDisarmed)
        {
            _isBeingArmed = true;

            sound.PlayOneShot(arming);
            ghost.SetWaitTime(_armingTime);
            _timer = _armingTime;
        }

        if (collision.CompareTag("Player") && _isArmed && !_isBeingDisarmed)
        {
            playerInRange = true;
            playerPrompt.SetActive(true);
            _timer = _disarmingTime;
            unarmingTimeUI.maxValue = _timer;
        }

        if (collision.CompareTag("Girlfriend") && _isArmed)
        {
            TriggerTrap();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isArmed && !_isBeingDisarmed) {

            playerPrompt.SetActive(false);
            playerInRange = false;
        } 
    }

    private void Update()
    {
        if (_timer != 0) { 

            
            if (_isBeingArmed) {
                _timerImage.DOFade(1, 0.1f);
                _timer -= Time.deltaTime;
            }
        
            if (playerInRange && Input.GetKey(KeyCode.E))
            {
                _isBeingDisarmed = true;
                _timer -= Time.deltaTime;

                unarmingTimeUI.value = _timer;
                timeRemaining.text = Mathf.FloorToInt(_timer + 1).ToString();
            }
        
        }

        if (_timer > 0)
        {
            if (_isBeingArmed)
            {
                var currentIncrement = _armingIncrements * (_currentSpriteIndex + 1);
                if (_timer < (_armingTime - currentIncrement))
                {
                    _currentSpriteIndex++;
                    SetTimerSprite(_currentSpriteIndex);
                }
            }
            //else
            //{
            //    var currentIncrement = _disarmingIncrements * (_currentSpriteIndex + 1);
            //    if (_timer < (_disarmingTime - currentIncrement))
            //    {
            //        _currentSpriteIndex++;
            //        SetTimerSprite(_currentSpriteIndex);
            //    }
            //}
        }


        if (_timer < 0)
        {
            if (_isBeingDisarmed)
            {
                playerPrompt.SetActive(false);
                _isBeingDisarmed = false;
                _isBeingArmed = false;
                _isArmed = false;

                //sound.PlayOneShot(removed);
                InteractionFinished();
            }
            else if (_isBeingArmed)
            {
                _isBeingArmed = false;
                _isBeingDisarmed = false;
                _isArmed = true;
                InteractionFinished();
            }
        }
    }

    private void TriggerTrap()
    {
        if (_isCleaver)
            transform.DOBlendableLocalMoveBy(new Vector3(0, -0.5f, 0), 0f).OnComplete(GameOver);
        else
            GameOver();
    }

    private void GameOver()
    {
        sound.pitch = .5f;
        sound.PlayOneShot(triggered);
        OnDeathTrapTriggered?.Invoke();
    }

    private void SetTimerSprite(int index)
    {
        if (index < _timerSprites.Count)
            _timerImage.sprite = _timerSprites[index];
    }

    private void InteractionFinished()
    {
        _timerImage.transform.DOShakeRotation(1f, 20);
        _timerImage.DOFade(0, 1f).OnComplete(ResetTimerImage);

        var renderer = GetComponent<SpriteRenderer>();

        if (_isArmed)
        {
            renderer.DOFade(1, 1f);
        }
        else
        {
            renderer.DOFade(0, 1f);
        }
    }

    private void ResetTimerImage()
    {
        _timerImage.sprite = _timerSprites[0];
    }

    #endregion


}
