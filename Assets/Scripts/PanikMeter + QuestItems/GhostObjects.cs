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

    [SerializeField] TextMeshProUGUI timeRemaining;
    [SerializeField] Slider slider;
    [SerializeField] GameObject canvas;

    public class TaskValues {
        public static int taskAngstValue1 = 10;
        public static int taskAngstValue2 = 25;
        public static int taskAngstValue3 = 50;

        public static float taskTime1 = 3f;
        public static float taskTime2 = 5f;
        public static float taskTime3 = 8f;
    }
    int fearValue = 0;

    public void PlayerInteraction() {
        OnGhostInteraction?.Invoke(this, EventArgs.Empty);
    }

    public void GhostInteraction() {
        OnGhostInteraction?.Invoke(this, EventArgs.Empty);
    }


    public void ChangeFearLevel(int fearChangeValue) {
        fearValue += fearChangeValue;
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController2D>() != null /* || Ghost */) {
            Debug.Log("TriggerEnter");

            canvas.SetActive(true);

            StartCoroutine(Timer(TaskValues.taskTime3));
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController2D>() != null) {
            Debug.Log("TriggerExit");

            canvas.SetActive(false);
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
        ChangeFearLevel(-5);
        Destroy(this.gameObject);
    }

}
