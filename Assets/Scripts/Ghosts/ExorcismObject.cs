using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ExorcismObject : MonoBehaviour
{
    #region Fields and Properties

    [SerializeField] private int _exorcismIndex; //links the object to its ghosts

    [SerializeField] private float _exorcismTime; //time it takes to excorcise object
    private float _timerIncrement; //time after which the next sprite is switched in
    private float _currentTimer;
    private int _currentSpriteIndex = 0;

    [SerializeField] private Image _timerImage;
    [SerializeField] private List<Sprite> _timerSprites = new();

    private bool _isInPlayerContact;
    const string PLAYER_TAG = "Player";

    private bool _isExcorcised;

    #endregion

    #region Methods

    private void Awake()
    {
        _timerIncrement = _exorcismTime / 6; //6 different sprites available
        _currentTimer = _exorcismTime;
    }

    private void Update()
    {
        if (!_isExcorcised)
        {
            //check if player tries to exorcise
            if (_isInPlayerContact && Input.GetKey(KeyCode.E))
            {
                _currentTimer -= Time.deltaTime;
                _timerImage.DOFade(1, 0.1f);
            }
            else
                _currentTimer = _exorcismTime;

            //handle timers for visuals
            if (_currentTimer == _exorcismTime)
            {
                _currentSpriteIndex = 0;
                SetTimerSprite(_currentSpriteIndex);
                _timerImage.DOFade(0, 0.1f);
            }
            else
            {
                var currentIncrement = _timerIncrement * (_currentSpriteIndex + 1);
                if (_currentTimer < (_exorcismTime - currentIncrement))
                {
                    _currentSpriteIndex++;
                    SetTimerSprite(_currentSpriteIndex);
                }
            }

            //finished exorcising
            if (_currentTimer <= 0)
                ObjectExorcised();
        }
    }

    private void SetTimerSprite(int index)
    {
        if (index < _timerSprites.Count)
            _timerImage.sprite = _timerSprites[index];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG))
            _isInPlayerContact = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG))
            _isInPlayerContact = false;
    }

    private void ObjectExorcised()
    {
        Ghost.RaiseExorcism(_exorcismIndex);
        _isExcorcised = true;
        var renderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        _timerImage.transform.DOShakeRotation(1f, 20);
        _timerImage.DOFade(0, 1f);
        renderer.DOFade(0, 1f).OnComplete(DestroyThisObject);
    }

    private void DestroyThisObject()
    {
        Destroy(gameObject);
    }

    #endregion
}
