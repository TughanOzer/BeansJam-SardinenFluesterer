using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExorciseObject : MonoBehaviour
{

    [SerializeField] WinLoseHandler winLoseHandler;
    public static event Action OnAllObjectsFound;

    public GameObject canvas;
    public Slider slider;
    [SerializeField] TextMeshProUGUI timeRemaining;
    public static int remainingObjects = 0;
    bool playerInRange = false;
    float unarmingTime = 3;
    float timer;

    private void OnEnable()
    {
        OnAllObjectsFound += AllObjectsFound;
    }
    private void OnDisable()
    {
        OnAllObjectsFound -= AllObjectsFound;
    }

    private void Start()
    {
        remainingObjects += 1;
        ResetExorcism();
        slider.maxValue = unarmingTime;
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.GetComponent<PlayerController2D>()) {
            canvas.SetActive(true);
            playerInRange = true;
            //StartCoroutine(Timer(3f));
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<PlayerController2D>())
        {
            canvas.SetActive(false);
            playerInRange = false;
            //StopCoroutine(Timer(0));
        }
    }
    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (timer == unarmingTime)
                {
                    slider.maxValue = unarmingTime;
                }
                timer -= Time.deltaTime;
                slider.value = timer;
                timeRemaining.text = ((int)timer+1).ToString();
                if (timer < 0)
                   ObjectExorcised();
            }
            else
                ResetExorcism();
        } 
    }
 
    void ResetExorcism()
    {
        timer = unarmingTime;
        slider.value = unarmingTime;
        timeRemaining.text = unarmingTime.ToString();
    }

    //IEnumerator Timer(float secondsleft) {
    //    float secondsleftValue = secondsleft;
    //    slider.maxValue = secondsleftValue;
    //    while ((int)secondsleft > -1) {
    //        if (Input.GetKey(KeyCode.E) && (int)secondsleft > 0) {
    //            secondsleft -= Time.deltaTime;
    //            slider.value = secondsleft;
    //        }
    //        else if (Input.GetKey(KeyCode.E) && (int)secondsleft == 0) ObjectExorcised();
    //        else if (!Input.GetKey(KeyCode.E)) {
    //            secondsleft = secondsleftValue;
    //            StopCoroutine(Timer(0));
    //        }
    //        yield return null;
    //    }
    //}

    void ObjectExorcised() {
        GameObject.FindObjectOfType<ObjectsFoundVisuals>().FadeImage(remainingObjects);

        remainingObjects--;
        if (remainingObjects == 0) 
            OnAllObjectsFound?.Invoke();

        Destroy(gameObject);
    }
    void AllObjectsFound() {

        winLoseHandler.SetMessage("You exorcised the poltergeist kids!");
    }

}
