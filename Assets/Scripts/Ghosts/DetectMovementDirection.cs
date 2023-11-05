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

        if (Mathf.Abs(difference.x) == 0.00f && Mathf.Abs(difference.y) == 0.00f)
        {
            animator.SetBool("moving", false);

            animator.SetFloat("Move x", 0);

            animator.SetFloat("Move y", 0);
        }
        else { 
            animator.SetBool("moving", true);
            
            if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
            {
                animator.SetFloat("Move x", difference.x);
                animator.SetFloat("Move y", 0);
            }
            else { 
                animator.SetFloat("Move y", difference.y);
                animator.SetFloat("Move x", 0);
            }
                
        }

        Debug.Log($"{gameObject.name}: {Mathf.Abs(difference.x)}, {Mathf.Abs(difference.y)}");
        lastPosition = currentPosition;

        Invoke("CheckMovement", .1f);
    }
}
