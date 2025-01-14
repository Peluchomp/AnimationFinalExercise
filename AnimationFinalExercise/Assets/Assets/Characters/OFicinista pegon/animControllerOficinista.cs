using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animControllerOficinista : MonoBehaviour
{
    private Animator animator;

    private int attackIndex = 0;
    private float lastAttackTime;
    private float attackResetTime = 2f; // Time after which the attack sequence resets

    public float movementSpeed = 2f; // Speed of character movement
    public float bigStepDistance = 3f; // Distance covered during a big step forward
    private bool isPerformingBigStep = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleAttacks();
    }

    void HandleMovement()
    {
        float moveDirection = 0f;

        if (Input.GetKey(KeyCode.L))
        {
            animator.SetBool("WalkingBackward", true);
            moveDirection = -1f; // Move backward
        }
        else
        {
            animator.SetBool("WalkingBackward", false);
        }

        if (Input.GetKey(KeyCode.J))
        {
            animator.SetBool("WalkingForward", true);
            moveDirection = 1f; // Move forward
        }
        else
        {
            animator.SetBool("WalkingForward", false);
        }

        // Move the character if not performing a big step
        if (!isPerformingBigStep)
        {
            transform.Translate(Vector3.forward * moveDirection * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("DodgeHighAttack");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            animator.SetTrigger("DodgeLowAttack");
        }

        if (Input.GetKeyDown(KeyCode.P) )
        {
            PerformSlowAttack();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            animator.SetTrigger("LowQuickAttack");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            animator.SetTrigger("LowSlowAttack");
        }

    }

    void PerformSlowAttack()
    {
        animator.SetTrigger("SlowAttackLongRange");     
    }

    void HandleAttacks()
    {
        if (Time.time - lastAttackTime > attackResetTime)
        {
            attackIndex = 0; // Reset the attack sequence after 2 seconds
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastAttackTime = Time.time; // Update last attack time

            animator.SetTrigger("QuickAttack");
            
            attackIndex = (attackIndex + 1) % 4; // Cycle through the sequence
        }
    }
}
