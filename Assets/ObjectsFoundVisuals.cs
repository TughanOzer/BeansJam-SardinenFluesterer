using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsFoundVisuals : MonoBehaviour
{
    #region Fields and Properties

    private List<Image> _ghosts = new();

    #endregion

    #region Methods

    private void Awake()
    {
        _ghosts = GetComponentsInChildren<Image>().ToList();
    }

    private void OnEnable()
    {
        Ghost.OnExorcism += FadeImage;
    }

    private void OnDisable()
    {
        Ghost.OnExorcism -= FadeImage;
    }

    private void FadeImage(int _)
    {
        if (_ghosts.Count > 0)
        {
            var image = _ghosts[_ghosts.Count - 1];
            _ghosts.Remove(image);
            image.DOFade(0.15f, 2).SetEase(Ease.InBounce);
            image.rectTransform.DOShakeRotation(2, 30, 2, 45, true, ShakeRandomnessMode.Harmonic);
        }
    }
    #endregion
}
