using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    bool showingRightSide;
    float verticalMovement;

    [Header("Ground Detection")]
    [Tooltip("Tip: Add an empty child object to the player and move it downwards with the moving tool, then add it to this field."),
     SerializeField] Transform groundDetector;
    float radius = 0.2f;
    [Tooltip("Create a new layer and assign it to the ground objects."), 
     SerializeField] LayerMask groundLayer;
    bool onGround;

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
        }
        else
            animator.SetBool("moving", false);

        // requiring float parameters to trigger the transition inside the animator (controller)
        animator.SetFloat("Move x", horizontalMovement);
        if(!sideScroller)
            animator.SetFloat("Move y", verticalMovement);

        //if(onGround) PlaySoundEffect(...);
        //else PlaySoundEffect(...);

        if (showingRightSide && horizontalMovement <0f || !showingRightSide && horizontalMovement >0f)
            Flip();
    }

    void Flip() {
        showingRightSide = !showingRightSide;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    

    void PlaySoundEffect(int x) {
        audioSource.clip = sounds[x];
        audioSource.Play();
    }

    //void OnCollisionEnter(Collision c)
    //{
    //    if (c.gameObject.CompareTag("Ground"))
    //        onGround = true;
    //}
}
