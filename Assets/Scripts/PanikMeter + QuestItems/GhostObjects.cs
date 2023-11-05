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
    [SerializeField] TextMeshProUGUI timeRemaining;
    [SerializeField] Slider slider;
    [SerializeField] GameObject canvas;
    Sprite standartSprite;
    SpriteRenderer spriteRenderer;

    [SerializeField] GOValues goValues;
    [SerializeField] AudioSource audioSource;

    int localFearValue;
    bool girlfriendInRange = false;
    bool objectIsHaunted = false;

    private void Start() {
        fearDisplay = FindObjectOfType<FearIdentifier>().gameObject.GetComponent<TextMeshProUGUI>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        standartSprite = spriteRenderer.sprite;
        
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.GetComponent<PlayerController2D>() && objectIsHaunted) {
            canvas.SetActive(true);
            StartCoroutine(Timer(goValues.taskTime));
        }
        else if (col.TryGetComponent(out Ghost ghost)) {
            //Temporär
            GhostInteraction();
            //
            ghost.SetWaitTime(goValues.taskTime);
        }
        else if (col.GetComponent<GirlfriendControllerEndo>()) {
            girlfriendInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.GetComponent<PlayerController2D>()) {
            canvas.SetActive(false);
            StopCoroutine(Timer(0));
        }
        else if (col.GetComponent<GirlfriendController>()) {
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

        audioSource.clip = goValues.playerCleaning;
        audioSource.Play();
    }
    public void ChangeFearLevel(int fearChangeValue) {
        localFearValue = fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu;
        localFearValue += fearChangeValue;
        // das hier raus
        //fearDisplay.text = "Fear: " + localFearValue;
        //fearDisplay.gameObject.GetComponent<FearIdentifier>().globalFearValu = localFearValue;
    }

    public void GhostInteraction() {
        objectIsHaunted = true;
        spriteRenderer.sprite = goValues.hauntedSprite;
        audioSource.clip = goValues.ghostUseSound;
        audioSource.Play();
        StartCoroutine(WaitingForGirlfriend());
    }

    IEnumerator WaitingForGirlfriend() {
        if (objectIsHaunted) {
            while (objectIsHaunted) {
                if (girlfriendInRange) {
                    ChangeFearLevel(goValues.taskAngstValue);
                    audioSource.clip = goValues.girlfriendScream;
                    audioSource.Play();

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
