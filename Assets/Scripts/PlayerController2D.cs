using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Game Mode")]
    [Tooltip("Check if you want to have jumping enabled, but vertical movement disabled."),
     SerializeField] bool sideScroller;
    public Vector2 moveDirection;

    [Header("Moving")]
    [Tooltip("Tip: Create and add a Physics Material to the Player's RB to prevent getting stuck."),
     SerializeField] Rigidbody2D playerRigidbody;
    [Tooltip("Insert a float value."),
     SerializeField] float movementSpeed;
    float horizontalMovement;
    float verticalMovement;

    [Header("Animation + Sound")]
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [Tooltip("Elements can be called by sounds[int x]."),
     SerializeField] List<AudioClip> sounds = new List<AudioClip>();

    void Update()
    {
        ProcessInputs();
    }
    private void FixedUpdate()
    {
        playerRigidbody.velocity = new Vector2(moveDirection.x * movementSpeed, moveDirection.y * movementSpeed);
    }


    void ProcessInputs()
    {
        // Axes are defined in Unity > Edit > Project Settings > Input Manager > Axes
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");
        Move();

        if (Input.GetKey(KeyCode.E)){ }
    }

    void Move() {

        moveDirection = new Vector2(horizontalMovement, verticalMovement);
        if (horizontalMovement != 0f || verticalMovement != 0f)
        {
            animator.SetBool("moving", true);
            PlaySoundEffect(0);
        }
        else
            animator.SetBool("moving", false);

        // requiring float parameters to trigger the transition inside the animator (controller)
        animator.SetFloat("Move x", horizontalMovement);

        if(!sideScroller)
            animator.SetFloat("Move y", verticalMovement);

    }

    void PlaySoundEffect(int x) {
        audioSource.clip = sounds[x];
        audioSource.Play();
    }

}
