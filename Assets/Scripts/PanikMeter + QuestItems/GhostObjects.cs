using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using TMPro;
using Unity.VisualScripting;

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

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController2D>() != null) {
            Debug.Log("TriggerEnter");
            canvas.SetActive(true);
            StartCoroutine(Timer(thisObjectTimeValue));
        }
        else if (collision.GetComponent<PlayerController2D>() != null) {

        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController2D>() != null) {
            Debug.Log("TriggerExit");
            canvas.SetActive(false);
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
                timeRemaining.text = seconds.ToString();
                slider.value = secondsleft;
            }
            else if (Input.GetKey(KeyCode.E) && secondsleft <= 0) TaskCompleted();
            else if (!Input.GetKey(KeyCode.E)) {
                Debug.Log("Reset Time");
                secondsleft = secondsleftValue;
                timeRemaining.text = secondsleft.ToString();
            }
            yield return new WaitForSeconds(0); //???
        }
    }

    void TaskCompleted() {
        Debug.Log("TaskDone");
        ChangeFearLevel(-thisObjectAngstValue);
        //item fade in, bisher destroy
        Destroy(this.gameObject);
    }
    public void ChangeFearLevel(int fearChangeValue) {
        fearValue = fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu;
        fearValue += fearChangeValue;
        fearDisplay.text = "Fear: " + fearValue;
        fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu = fearValue;
    }
    public void PlayerInteraction() {
        
    }

    public void GhostInteraction() {
        ChangeFearLevel(thisObjectAngstValue);

        // item fade out

    }

}
