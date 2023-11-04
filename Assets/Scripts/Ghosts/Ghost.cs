using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Search;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    #region Fields and Properties

    public static event Action<int> OnExorcism;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _floatTime = 1f;
    [SerializeField] private float _floatAmplitude = 1f;
    [SerializeField] private Ease _floatEase = Ease.Linear;


    private int _objectIndex;
    private List<GameObject> _possibleInteractions = new();
    private GameObject _nextInteractionObject;
    private bool _isAtTargetObject;
    private Transform _visuals;

    const string INTERACTABLE_OBJECT_TAG = "InteractableObject";
    
    #endregion

    #region Methods

    private void OnEnable()
    {
        OnExorcism += Exorcise;
    }

    private void OnDisable()
    {
        OnExorcism -= Exorcise;
    }

    private void Awake()
    {
        _visuals = transform.GetChild(0);
    }

    private void Start()
    {
        SetInteractionTarget();
        _visuals.DOLocalMoveY(_floatAmplitude, _floatTime).SetEase(_floatEase).OnComplete(GhostWalkDown);
    }

    private void GhostWalkUp()
    {
        _visuals.DOLocalMoveY(_floatAmplitude, _floatTime).SetEase(_floatEase).OnComplete(GhostWalkDown);
    }

    private void GhostWalkDown()
    {
        _visuals.DOLocalMoveY(-_floatAmplitude, _floatTime).SetEase(_floatEase).OnComplete(GhostWalkUp);
    }

    //tells the ghost which object it's connected to
    public void SetUp(int objectIndex)
    {
        _objectIndex = objectIndex;        
    }

    private void Update()
    {
        var nextInteractionPosition = _nextInteractionObject.transform.position;
        var distance = Vector3.Distance(transform.position, nextInteractionPosition);

        if (distance > 1f)
        {
            _isAtTargetObject = false;
            var direction = nextInteractionPosition - transform.position;
            transform.Translate(direction.normalized * _speed);
        }
        else
        {
            _isAtTargetObject = true;
        }        
    }



    private void SetInteractionTarget()
    {
        _possibleInteractions = GameObject.FindGameObjectsWithTag(INTERACTABLE_OBJECT_TAG).ToList();

        var randomInteractionIndex = UnityEngine.Random.Range(0, _possibleInteractions.Count);
        _nextInteractionObject = _possibleInteractions[randomInteractionIndex];
    }

    private void Exorcise(int objectIndex)
    {
        if (objectIndex == _objectIndex)
        {
            //Leave screen and disappear
        }
    }



    #endregion
}
