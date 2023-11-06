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

        var absDiffX = Mathf.Abs(difference.x);
        var absDiffY = Mathf.Abs(difference.y);

        if (absDiffX == 0.00f && absDiffY == 0.00f)
        {
            animator.SetBool("moving", false);

            animator.SetFloat("Move x", 0);

            animator.SetFloat("Move y", 0);
        }
        else { 
            animator.SetBool("moving", true);
            
            if (absDiffX > absDiffY)
            {
                if (difference.x < 0.0f)
                animator.SetFloat("Move x", -1);
                if (difference.x > 0.0f)
                 animator.SetFloat("Move x", +1);

                animator.SetFloat("Move y", 0);
            }
            else {
                if (difference.y < 0.0f)
                    animator.SetFloat("Move y", -1);
                if (difference.y > 0.0f)
                    animator.SetFloat("Move y", +1);
                animator.SetFloat("Move x", 0);
            }
                
        }

        //Debug.Log($"{gameObject.name}: {Mathf.Abs(difference.x)}, {Mathf.Abs(difference.y)}");
        lastPosition = currentPosition;

        Invoke("CheckMovement", .1f);
    }
}
