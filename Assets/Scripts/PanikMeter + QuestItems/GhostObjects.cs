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

    [SerializeField] TextMeshProUGUI fearDisplay;
    [SerializeField] TextMeshProUGUI timeRemaining;
    [SerializeField] Slider slider;
    [SerializeField] GameObject canvas;

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
    [SerializeField] bool objectIsHaunted = false;

    private void Start() {
        fearDisplay = FindObjectOfType<FearIdentifier>().gameObject.GetComponent<TextMeshProUGUI>();
        thisObjectAngstValue = StartupAngstValues();
        thisObjectTimeValue = StartupTimeValues();
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

        Debug.Log("TriggerEnter");
        if (collision.GetComponent<PlayerController2D>() && objectIsHaunted) {
            canvas.SetActive(true);
            StartCoroutine(Timer(thisObjectTimeValue));
        }
        else if (collision.GetComponent<Ghost>()) {
            //Evtl nicht nötig
        }
        else if (collision.GetComponent<GirlfriendController>()) {
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
        Debug.Log("TaskDone");
        ChangeFearLevel(-thisObjectAngstValue);
        objectIsHaunted = false;
        StartCoroutine(FadeObjectOut(false));
    }
    public void ChangeFearLevel(int fearChangeValue) {
        fearValue = fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu;
        fearValue += fearChangeValue;
        fearDisplay.text = "Fear: " + fearValue;
        fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu = fearValue;
    }

    public void GhostInteraction() {
        objectIsHaunted = true;
        StartCoroutine(FadeObjectOut(true));
        //StartCoroutine(WaitingForGirlfriend());
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
                else yield return null;
            }
        }
    }

    [SerializeField] float fadeOutTime = 2f;
    [SerializeField] float fadeOutMin = 0.25f;
    IEnumerator FadeObjectOut(bool fadeOut) {
        Debug.Log("Fadeout1");
        SpriteRenderer sprite = this.gameObject.GetComponent<SpriteRenderer>();
        Color color = sprite.color;

        //FadeIN
        if (!fadeOut) {
            Debug.Log("fadeInMin0: " + color.a);
            while (color.a < 1f) {
                color.a += Time.deltaTime / fadeOutTime;
                sprite.color = color;
            }
        }

        //FadeOUT
        else {
            Debug.Log("FadeOUT2");
            while (color.a >= fadeOutMin) {
                Debug.Log("FadeIn: " + color.a);
                color.a -= Time.deltaTime / fadeOutTime;
                this.gameObject.GetComponent<SpriteRenderer>().color = color;
            }
        }
        yield return null;
    }


}
