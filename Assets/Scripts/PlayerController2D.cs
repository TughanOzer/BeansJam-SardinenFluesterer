using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Moving")]
    [Tooltip("Tip: Create and add a Physics Material to the Player's RB to prevent getting stuck."),
     SerializeField] Rigidbody2D playerRigidbody;
    [Tooltip("Insert a float value."),
     SerializeField] float movementSpeed;
    float horizontalMovement;
    float verticalMovement;
    bool moving;
    public Vector2 moveDirection;

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

        //if(moving) PlaySoundEffect(0);
    }

    void Move() {

        moveDirection = new Vector2(horizontalMovement, verticalMovement);
        if (horizontalMovement != 0f || verticalMovement != 0f)
        {
            moving = true;
            animator.SetBool("moving", true);

            
            // requiring float parameters to trigger the transition inside the animator (controller)
            if (Mathf.Abs(horizontalMovement) > Mathf.Abs(verticalMovement))
            {
                if (horizontalMovement < 0.0f)
                    animator.SetFloat("Move x", -1);
                if (horizontalMovement > 0.0f)
                    animator.SetFloat("Move x", +1);
                animator.SetFloat("Move y", 0);
            }
            else
            {
                if (verticalMovement < 0.0f)
                    animator.SetFloat("Move y", -1);
                if (verticalMovement > 0.0f)
                    animator.SetFloat("Move y", +1);
                animator.SetFloat("Move x", 0);
            }

        }
        else
        {
            animator.SetBool("moving", false);
            moving = false;
            animator.SetFloat("Move y", 0);
            animator.SetFloat("Move x", 0);
        }

        
    }

    void PlaySoundEffect(int x) {
        audioSource.clip = sounds[x];
        audioSource.Play();
    }

}
