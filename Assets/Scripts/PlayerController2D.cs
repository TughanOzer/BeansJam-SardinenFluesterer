using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Game Mode")]
    [Tooltip("Check if you want to have jumping enabled, but vertical movement disabled."),
     SerializeField] bool sideScroller;
    Vector2 moveDirection;

    [Header("Moving")]
    [Tooltip("Tip: Create and add a Physics Material to the Player's RB to prevent getting stuck."),
     SerializeField] Rigidbody2D playerRigidbody;
    [Tooltip("Insert a float value."),
     SerializeField] float movementSpeed;
    float horizontalMovement;
    bool showingRightSide;
    float verticalMovement;

    [Header("Jumping")]
    [Tooltip("Insert a float value."),
     SerializeField] float jumpForce;
    [Tooltip("Insert a float value."),
     SerializeField] float gravityModifier;
    int jumpCount;
    [Tooltip("Insert an integer value."),
     SerializeField] int maxJumpCount;

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


    void Start() 
    {
        Physics.gravity *= gravityModifier;
        jumpCount = maxJumpCount;

        // preventing the player from spinning
        playerRigidbody.freezeRotation = true;
    }

    void Update()
    {
        ProcessInputs();
    }
    private void FixedUpdate()
    {
        if (sideScroller) {
            playerRigidbody.velocity = new Vector2(horizontalMovement * movementSpeed, playerRigidbody.velocity.y);
            onGround = Physics2D.OverlapCircle(groundDetector.position, radius, groundLayer);
            if (onGround)
            {
                jumpCount = maxJumpCount;
                animator.SetBool("IsJumping", false);
            }
        }
        else
        {
            playerRigidbody.velocity = new Vector2(moveDirection.x * movementSpeed, moveDirection.y * movementSpeed);
        }
    }


    void ProcessInputs()
    {
        // Axes are defined in Unity > Edit > Project Settings > Input Manager > Axes
        horizontalMovement = Input.GetAxisRaw("Horizontal");

        if (!sideScroller)
        {
            verticalMovement = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector2(horizontalMovement, verticalMovement);

        }
        else {
            if (horizontalMovement != 0f)
                MoveHorizontal();

            // Keycodes: https://docs.unity3d.com/ScriptReference/KeyCode.html
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (jumpCount > 0)
                    Jump();
            }
        }
    }

    void MoveHorizontal() {

        // requires a float parameter called Speed to trigger the transition inside the animator (controller)
        animator.SetFloat("Speed", Mathf.Abs(horizontalMovement));

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
    

    void Jump()
    {
        if (jumpCount > 0)
        {
            animator.SetBool("IsJumping", true);
            //PlaySoundEffect(...);

            playerRigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            jumpCount--;
        }
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
