using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectMovementDirection : MonoBehaviour
{

    Vector3 lastPosition;
    [SerializeField] Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        CheckMovement();
    }

    void CheckMovement()
    {
        var currentPosition = transform.position;
        var difference = currentPosition - lastPosition;

        if (difference.x == 0.0f && difference.y == 0.0f)
        {
            animator.SetBool("moving", false);
        }
        else { 
            animator.SetBool("moving", true);
        }
           
            animator.SetFloat("Move x", difference.x);
            animator.SetFloat("Move y", difference.y);

        lastPosition = currentPosition;

        Invoke("CheckMovement", .1f);
    }
}
