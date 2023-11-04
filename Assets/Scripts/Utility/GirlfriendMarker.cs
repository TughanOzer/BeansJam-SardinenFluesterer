using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GirlfriendMarker : MonoBehaviour
{
    #region Fields and Properties

    private Transform _targetTransform;
    private RectTransform _rectTransform;
    private Image _icon;

    #endregion

    #region Methods

    private void Start()
    {
        _targetTransform = GameObject.FindWithTag("Girlfriend").transform;
        _rectTransform = GetComponent<RectTransform>();
        _icon = GetComponent<Image>();
    }

    private void Update()
    {
        var targetPosition = _targetTransform.position;

        float borderSize = 20f;
        var targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || 
            targetPositionScreenPoint.x >= Screen.width - borderSize ||
            targetPositionScreenPoint.y <= borderSize ||
            targetPositionScreenPoint.y >= Screen.height - borderSize;

        if (isOffScreen)
        {
            _icon.enabled = true;
            var cappedTargetScreenPosition = targetPositionScreenPoint;

            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
            if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

            Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(cappedTargetScreenPosition);
            _rectTransform.position = cappedTargetScreenPosition;
            _rectTransform.localPosition = new(_rectTransform.localPosition.x, _rectTransform.localPosition.y, 0);
        }
        else
        {
            _icon.enabled = false;
        }
    }

    #endregion
}
