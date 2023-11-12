using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsFoundVisuals : MonoBehaviour
{
    #region Fields and Properties

    public List<Image> GhostImages { get; private set; } = new();

    #endregion

    #region Methods

    private void Awake()
    {
        GhostImages = GetComponentsInChildren<Image>().ToList();
    }

    //private void OnEnable()
    //{
    //    Ghost.OnExorcism += FadeImage;
    //}

    //private void OnDisable()
    //{
    //    Ghost.OnExorcism -= FadeImage;
    //}

    public void FadeImage(int _)
    {
        if (GhostImages.Count > 0)
        { 
            Debug.Log("ghost images before " + GhostImages.Count);
            if (FindObjectOfType<GhostManager>().ghostsInGame != 0)
            {
                foreach (var ghost in FindObjectsOfType<Ghost>())
                    ghost.Exorcise(_);
            }
            var image = GhostImages[GhostImages.Count - 1];
            GhostImages.Remove(image);
            image.DOFade(0.15f, 2).SetEase(Ease.InBounce);
            image.rectTransform.DOShakeRotation(2, 30, 2, 45, true, ShakeRandomnessMode.Harmonic);
            Debug.Log("ghost images after " + GhostImages.Count);
        }
        
    }
    #endregion
}
