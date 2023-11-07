using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemporaryWin : MonoBehaviour
{

    [SerializeField] WinLoseHandler winLoseHandler;

    public GameObject canvas;
    public Slider slider;



    private void OnTriggerEnter2D(Collider2D col) {
        if (col.GetComponent<PlayerController2D>()) {
            canvas.SetActive(true);
            StartCoroutine(Timer(3f));
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<PlayerController2D>())
        {
            canvas.SetActive(false);
            slider.value = 3;
            StopCoroutine(Timer(0));
        }
    }

    IEnumerator Timer(float secondsleft) {
        float secondsleftValue = secondsleft;
        slider.maxValue = secondsleftValue;
        while (secondsleft >= -1) {
            if (Input.GetKey(KeyCode.E) && secondsleft > 0) {
                float seconds = Mathf.FloorToInt(secondsleft);
                secondsleft -= Time.deltaTime;
                slider.value = secondsleft;
            }
            else if (Input.GetKey(KeyCode.E) && secondsleft <= 0) Win();
            else if (!Input.GetKey(KeyCode.E)) {
                secondsleft = secondsleftValue;
            }
            yield return null;
        }
    }

    void Win() {
        //fearmeter.GetComponent<FearMeter>.().OnHappinessMax?.Invoke();
        winLoseHandler.FadeInWinImage();
    }

}
