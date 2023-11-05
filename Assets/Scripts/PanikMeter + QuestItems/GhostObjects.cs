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
    [SerializeField] Sprite hauntedSprite;
    [SerializeField] Sprite standartSprite;
    SpriteRenderer spriteRenderer;

    [SerializeField] enum TaskFearValues { taskValueAngst10, taskValueAngst25, taskValueAngst50 }
    [SerializeField] TaskFearValues taskFearValue;
    [SerializeField] enum TaskTimeValues { taskValueTime1, taskValueTime2, taskValueTime3 }
    [SerializeField] TaskTimeValues taskTimeValue;

    public class TaskValues {
        public static int taskAngstValue10 = 10, taskAngstValue25 = 25, taskAngstValue35 = 50;
        public static float taskTime1 = 3f, taskTime2 = 5f, taskTime3 = 8f;
    }
    int fearValue;
    int thisObjectAngstValue;
    float thisObjectTimeValue;
    bool objectIsHaunted = false;

    private void Start() {
        fearDisplay = FindObjectOfType<FearIdentifier>().gameObject.GetComponent<TextMeshProUGUI>();
        thisObjectAngstValue = StartupAngstValues();
        thisObjectTimeValue = StartupTimeValues();
        standartSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    int StartupAngstValues() {
        switch (taskFearValue) {
            case TaskFearValues.taskValueAngst10:
                return TaskValues.taskAngstValue10;
            case TaskFearValues.taskValueAngst25: 
                return TaskValues.taskAngstValue25;
            case TaskFearValues.taskValueAngst50:
                return TaskValues.taskAngstValue35;
            default:
                return 0;
        }
    }

    float StartupTimeValues() {
        switch (taskTimeValue) {
            case TaskTimeValues.taskValueTime1:
                return TaskValues.taskTime1;
            case TaskTimeValues.taskValueTime2:
                return TaskValues.taskTime2;
            case TaskTimeValues.taskValueTime3:
                return TaskValues.taskTime3;
            default:
                return 0;
        }
    }

    bool girlfriendInRange = false;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController2D>() && objectIsHaunted) {
            canvas.SetActive(true);
            StartCoroutine(Timer(thisObjectTimeValue));
        }
        else if (collision.TryGetComponent(out Ghost ghost)) {
            ghost.SetWaitTime(thisObjectTimeValue);
        }
        else if (collision.GetComponent<GirlfriendControllerEndo>()) {
            Debug.Log("Girlfriend in Range");
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
        ChangeFearLevel(-thisObjectAngstValue);
        objectIsHaunted = false;
        //StartCoroutine(FadeObjectOut(false));
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
        spriteRenderer.sprite = hauntedSprite;
        //StartCoroutine(FadeObjectOut(true));
        StartCoroutine(WaitingForGirlfriend());
    }

    IEnumerator WaitingForGirlfriend() {
        if (objectIsHaunted) {
            while (objectIsHaunted) {
                if (girlfriendInRange) {
                    ChangeFearLevel(thisObjectAngstValue);
                    StartCoroutine(FadeObjectOut(false));
                    objectIsHaunted = false;
                    girlfriendInRange = false;
                    yield return null;
                } 
                else
                    //Debug.Log("Girlfriend Not Range");
                yield return null;
            }
        }
    }

    [SerializeField] float fadeOutTime = 2f;
    [SerializeField] float fadeOutMin = 0.25f;
    IEnumerator FadeObjectOut(bool fadeOut) {
        SpriteRenderer sprite = this.gameObject.GetComponent<SpriteRenderer>();
        Color color = sprite.color;
        float alpha = color.a;

        if(fadeOut) {
            for (float t = 0.0f; t < 1f; t += Time.deltaTime / fadeOutTime) {

                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, fadeOutMin, t));
                this.gameObject.GetComponent<SpriteRenderer>().material.color = newColor;
            }
        }
        else {
            for (float t = 0.0f; t < 1f; t += Time.deltaTime / fadeOutTime) {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 1f, t));
                this.gameObject.GetComponent<SpriteRenderer>().material.color = newColor;
            }
        }

        //if (!fadeOut) {
        //    while (color.a < 1f) {
        //        color.a += Time.deltaTime / fadeOutTime;
        //        sprite.color = color;
        //    }
        //}
        //else {
        //    while (color.a >= fadeOutMin) {
        //        color.a -= Time.deltaTime / fadeOutTime;
        //        this.gameObject.GetComponent<SpriteRenderer>().color = color;
        //    }
        //}

        yield return null;
    }

}
