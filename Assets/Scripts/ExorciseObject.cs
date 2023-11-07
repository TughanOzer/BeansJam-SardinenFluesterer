using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class ExorciseObject : MonoBehaviour
{

    [SerializeField] WinLoseHandler winLoseHandler;

    public GameObject canvas;
    public Slider slider;
    [SerializeField] TextMeshProUGUI timeRemaining;
    public static int remainingObjects = 0;
    bool playerInRange = false;
    float unarmingTime;

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
                unarmingTime = unarmingTime - Time.deltaTime;
                slider.value = unarmingTime;
                timeRemaining.text = unarmingTime.ToString();
                if ((int)unarmingTime == 0)
                   ObjectExorcised();
            }
            else
                ResetExorcism();
        } 
    }
 
    void ResetExorcism()
    {
        unarmingTime = 3;
        slider.value = 3;
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
        if(TryGetComponent(out Ghost ghost)) 
            ghost.Exorcise(remainingObjects);
        remainingObjects--;
        if (remainingObjects == 0) {
            winLoseHandler.FadeInWinImage();
            winLoseHandler.SetMessage("You exorcised the poltergeist kids!");
        }
        Destroy(gameObject);
    }
   

}
