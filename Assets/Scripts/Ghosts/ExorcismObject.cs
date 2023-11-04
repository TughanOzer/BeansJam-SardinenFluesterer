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


    #endregion

    #region Methods

    private void Awake()
    {
        _timerIncrement = _exorcismTime / 6; //6 different sprites available
        _currentTimer = _exorcismTime;
    }

    private void Update()
    {
        if (_isInPlayerContact && Input.GetKey(KeyCode.E))
        {
            _currentTimer -= Time.deltaTime;
        }

        if (_currentTimer == _exorcismTime)
        {
            _currentSpriteIndex = 0;
            SetTimerSprite(_currentSpriteIndex);          
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
    }

    #endregion
}
