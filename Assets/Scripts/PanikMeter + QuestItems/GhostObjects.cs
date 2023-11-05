using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using TMPro;

public class GhostObjects : MonoBehaviour
{
    public event EventHandler OnGhostInteraction;

    TextMeshProUGUI fearDisplay;
    [SerializeField] TextMeshProUGUI timeRemaining;
    [SerializeField] Slider slider;
    [SerializeField] GameObject canvas;
    Sprite standartSprite;
    SpriteRenderer spriteRenderer;

    [SerializeField] GOValues goValues;

    int fearValue;
    bool objectIsHaunted = false;

    private void Start() {
        fearDisplay = FindObjectOfType<FearIdentifier>().gameObject.GetComponent<TextMeshProUGUI>();
        standartSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    bool girlfriendInRange = false;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController2D>() && objectIsHaunted) {
            canvas.SetActive(true);
            StartCoroutine(Timer(goValues.taskTime));
        }
        else if (collision.TryGetComponent(out Ghost ghost)) {
            ghost.SetWaitTime(goValues.taskTime);
        }
        else if (collision.GetComponent<GirlfriendControllerEndo>()) {
            girlfriendInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController2D>()) {
            canvas.SetActive(false);
            StopCoroutine(Timer(0));
        }
        else if (collision.GetComponent<GirlfriendController>()) {
            girlfriendInRange = false;
        }
    }

    IEnumerator Timer(float secondsleft) {
        float secondsleftValue = secondsleft;
        slider.maxValue = secondsleftValue;
        while (secondsleft >= -1 && objectIsHaunted) {
            if (Input.GetKey(KeyCode.E) && secondsleft > 0) {
                float seconds = Mathf.FloorToInt(secondsleft);
                secondsleft -= Time.deltaTime;
                timeRemaining.text = seconds.ToString();
                slider.value = secondsleft;
            }
            else if (Input.GetKey(KeyCode.E) && secondsleft <= 0) TaskCompleted();
            else if (!Input.GetKey(KeyCode.E)) {
                secondsleft = secondsleftValue;
                timeRemaining.text = secondsleft.ToString();
            }
            yield return null;
        }
    }

    void TaskCompleted() {
        ChangeFearLevel(-goValues.taskAngstValue);
        objectIsHaunted = false;
        canvas.SetActive(false);
        spriteRenderer.sprite = standartSprite;

    }
    public void ChangeFearLevel(int fearChangeValue) {
        fearValue = fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu;
        fearValue += fearChangeValue;
        fearDisplay.text = "Fear: " + fearValue;
        fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu = fearValue;
    }

    public void GhostInteraction() {
        objectIsHaunted = true;
        spriteRenderer.sprite = goValues.hauntedSprite;
        StartCoroutine(WaitingForGirlfriend());
    }

    IEnumerator WaitingForGirlfriend() {
        if (objectIsHaunted) {
            while (objectIsHaunted) {
                if (girlfriendInRange) {
                    ChangeFearLevel(goValues.taskAngstValue);
                    objectIsHaunted = false;
                    spriteRenderer.sprite = standartSprite;
                    girlfriendInRange = false;
                    yield return null;
                } 
                else yield return null;
            }
        }
    }

}
