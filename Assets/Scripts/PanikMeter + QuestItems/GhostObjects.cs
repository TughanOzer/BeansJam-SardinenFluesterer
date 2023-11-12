using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using TMPro;


[RequireComponent(typeof(AudioSource))]
public class GhostObjects : MonoBehaviour
{
    public event EventHandler OnGhostInteraction;

    TextMeshProUGUI fearDisplay;
    float timer;
    [SerializeField] TextMeshProUGUI timeRemaining;
    [SerializeField] Slider slider;
    [SerializeField] GameObject canvas;
    Sprite standardSprite;
    SpriteRenderer spriteRenderer;

    [SerializeField] GOValues goValues;
    [SerializeField] AudioSource audioSource;

    public int localFearValue;
    bool girlfriendInRange = false;
    bool girlfriendShocked = false;
    bool objectIsHaunted = false;
    bool playerInRange = false;

    private void Start() {
        fearDisplay = FindObjectOfType<FearIdentifier>().gameObject.GetComponent<TextMeshProUGUI>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        standardSprite = spriteRenderer.sprite;
        slider.maxValue = goValues.taskTime;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.GetComponent<PlayerController2D>()) {
            playerInRange = true;
            if (objectIsHaunted) timer = goValues.taskTime;

            //StartCoroutine(Timer(goValues.taskTime));
        }
        else if (col.TryGetComponent(out Ghost ghost) && !objectIsHaunted) {
            //Temporäre Notlösung
            GhostInteraction();
            //
            ghost.SetWaitTime(goValues.taskTime);
        }
        else if (col.GetComponent<GirlfriendControllerEndo>()) {
            girlfriendInRange = true;
        }
    }
  
    private void Update()
    {
        if (objectIsHaunted)
        {
            if (girlfriendInRange && !girlfriendShocked)
            {
                ChangeFearLevel(-goValues.taskAngstValue);
                audioSource.PlayOneShot(goValues.girlfriendScream);
                girlfriendShocked = true;
            }

            if (playerInRange)
            {
                canvas.SetActive(true);
                
                if (Input.GetKey(KeyCode.E))
                {   
                    if (timer == goValues.taskTime)
                    {
                        slider.maxValue = timer;
                        audioSource.PlayOneShot(goValues.playerCleaning);
                    }
                    timer -= Time.deltaTime;
                    timeRemaining.text = ((int)timer + 1).ToString();
                    slider.value = timer;
                    if (timer < 0)
                        TaskCompleted();
                }
                else
                    ResetCleaning();
            }else canvas.SetActive(false);
        }
    }
    void ResetCleaning()
    {
        timer = goValues.taskTime;
        slider.value = timer;
        timeRemaining.text = ((int)timer+1).ToString();
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.GetComponent<GirlfriendControllerEndo>())
        {
            girlfriendInRange = false;
            girlfriendShocked = false;
        }
        if(col.GetComponent<PlayerController2D>())
            playerInRange = false;
    }

    //IEnumerator Timer(float secondsleft) {
    //    float secondsleftValue = secondsleft;
    //    slider.maxValue = secondsleftValue;
    //    while (secondsleft >= -1 && objectIsHaunted) {
    //        if (Input.GetKey(KeyCode.E) && secondsleft > 0) {
    //            float seconds = Mathf.FloorToInt(secondsleft);
    //            secondsleft -= Time.deltaTime;
    //            timeRemaining.text = (seconds +1).ToString();
    //            slider.value = secondsleft;
    //            audioSource.clip = goValues.playerCleaning;
    //            audioSource.Play();
    //        }
    //        else if (Input.GetKey(KeyCode.E) && secondsleft <= 0) TaskCompleted();
    //        else if (!Input.GetKey(KeyCode.E)) {
    //            secondsleft = secondsleftValue;
    //            timeRemaining.text = (secondsleft +1).ToString();
    //            audioSource.Stop();
    //        }
    //        yield return null;
    //    }
    //}

    void TaskCompleted() {
        ChangeFearLevel(goValues.taskAngstValue);
        objectIsHaunted = false;
        canvas.SetActive(false);
        spriteRenderer.sprite = standardSprite;

        //audioSource.clip = goValues.playerCleaning;
        //audioSource.Play();
    }
    public void ChangeFearLevel(int fearChangeValue) {
        localFearValue = fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu;
        localFearValue += fearChangeValue;
        Debug.Log(localFearValue);
        fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu = localFearValue;
    }

    public void GhostInteraction() {
        spriteRenderer.sprite = goValues.hauntedSprite;
        audioSource.clip = goValues.ghostUseSound;
        audioSource.Play();
        objectIsHaunted = true;
        //StartCoroutine(WaitingForGirlfriend());
    }

    //IEnumerator WaitingForGirlfriend() {
    //    if (objectIsHaunted) {
    //        while (objectIsHaunted) {
    //            if (girlfriendInRange && !girlfriendShocked) {
    //                ChangeFearLevel(goValues.taskAngstValue);
    //                audioSource.clip = goValues.girlfriendScream;
    //                audioSource.Play();

    //                //objectIsHaunted = false;
    //                //spriteRenderer.sprite = standartSprite;
    //                girlfriendShocked = true;
    //                yield return null;
    //            } 
    //            else yield return null;
    //        }
    //    }
    //}

}
