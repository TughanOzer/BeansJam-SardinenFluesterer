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
    Sprite standardSprite;
    SpriteRenderer spriteRenderer;

    [SerializeField] GOValues goValues;
    [SerializeField] AudioSource audioSource;

    public int localFearValue;
    bool girlfriendInRange = false;
    bool girlfriendShocked = false;
    bool objectIsHaunted = false;

    private void Start() {
        fearDisplay = FindObjectOfType<FearIdentifier>().gameObject.GetComponent<TextMeshProUGUI>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        standardSprite = spriteRenderer.sprite;
        
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.GetComponent<PlayerController2D>() && objectIsHaunted) {
            canvas.SetActive(true);
            StartCoroutine(Timer(goValues.taskTime));
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
       // Debug.Log($"{ gameObject.name} was triggered by { col.gameObject.name}");
    }

    private void Update()
    {
        if (girlfriendInRange)
        {
            if (objectIsHaunted && !girlfriendShocked)
            {
                ChangeFearLevel(-goValues.taskAngstValue);
                audioSource.clip = goValues.girlfriendScream;
                audioSource.PlayOneShot(audioSource.clip);
                girlfriendShocked = true;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.GetComponent<PlayerController2D>() && objectIsHaunted) {
            canvas.SetActive(false);
            slider.value = goValues.taskTime;
            StopCoroutine(Timer(0));
        }
        else if (col.GetComponent<GirlfriendControllerEndo>())
        {
            girlfriendInRange = false;
            girlfriendShocked = false;
        }
        // Debug.Log($"{gameObject.name} got left by {col.gameObject.name}");
    }

    IEnumerator Timer(float secondsleft) {
        float secondsleftValue = secondsleft;
        slider.maxValue = secondsleftValue;
        while (secondsleft >= -1 && objectIsHaunted) {
            if (Input.GetKey(KeyCode.E) && secondsleft > 0) {
                float seconds = Mathf.FloorToInt(secondsleft);
                secondsleft -= Time.deltaTime;
                timeRemaining.text = (seconds +1).ToString();
                slider.value = secondsleft;
                audioSource.clip = goValues.playerCleaning;
                audioSource.Play();
            }
            else if (Input.GetKey(KeyCode.E) && secondsleft <= 0) TaskCompleted();
            else if (!Input.GetKey(KeyCode.E)) {
                secondsleft = secondsleftValue;
                timeRemaining.text = (secondsleft +1).ToString();
                audioSource.Stop();
            }
            yield return null;
        }
    }

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
        objectIsHaunted = true;
        spriteRenderer.sprite = goValues.hauntedSprite;
        audioSource.clip = goValues.ghostUseSound;
        audioSource.Play();
        //StartCoroutine(WaitingForGirlfriend());
    }

    IEnumerator WaitingForGirlfriend() {
        if (objectIsHaunted) {
            while (objectIsHaunted) {
                if (girlfriendInRange && !girlfriendShocked) {
                    ChangeFearLevel(goValues.taskAngstValue);
                    audioSource.clip = goValues.girlfriendScream;
                    audioSource.Play();

                    //objectIsHaunted = false;
                    //spriteRenderer.sprite = standartSprite;
                    girlfriendShocked = true;
                    yield return null;
                } 
                else yield return null;
            }
        }
    }

}
