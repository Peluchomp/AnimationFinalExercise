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
    private bool isAttackingOrDodging = false; // Flag to check if the player is attacking or dodging

    private float attackCooldown = 1f; // Cooldown time between attacks
    private float dodgeCooldown = 1f; // Cooldown time between dodges
    private float lastAttackOrDodgeTime;

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
        if (isAttackingOrDodging) return; // Prevent movement if attacking or dodging

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

        if (Input.GetKeyDown(KeyCode.K) && Time.time - lastAttackOrDodgeTime > dodgeCooldown)
        {
            animator.SetTrigger("DodgeHighAttack");
            lastAttackOrDodgeTime = Time.time;
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));
        }

        if (Input.GetKeyDown(KeyCode.I) && Time.time - lastAttackOrDodgeTime > dodgeCooldown)
        {
            animator.SetTrigger("DodgeLowAttack");
            lastAttackOrDodgeTime = Time.time;
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));
        }

        if (Input.GetKeyDown(KeyCode.P) && Time.time - lastAttackOrDodgeTime > attackCooldown)
        {
            animator.SetTrigger("SlowAttackLongRange");
            lastAttackOrDodgeTime = Time.time;
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));
        }

        if (Input.GetKeyDown(KeyCode.U) && Time.time - lastAttackOrDodgeTime > attackCooldown)
        {
            animator.SetTrigger("LowQuickAttack");
            lastAttackOrDodgeTime = Time.time;
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));
        }

        if (Input.GetKeyDown(KeyCode.M) && Time.time - lastAttackOrDodgeTime > attackCooldown)
        {
            animator.SetTrigger("LowSlowAttack");
            lastAttackOrDodgeTime = Time.time;
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));
        }
    }

    void HandleAttacks()
    {
        if (Time.time - lastAttackTime > attackResetTime)
        {
            attackIndex = 0; // Reset the attack sequence after 2 seconds
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastAttackOrDodgeTime > attackCooldown)
        {
            lastAttackTime = Time.time; // Update last attack time
            lastAttackOrDodgeTime = Time.time; // Update last attack or dodge time

            animator.SetTrigger("QuickAttack");
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));

            attackIndex = (attackIndex + 1) % 4; // Cycle through the sequence
        }
    }

    IEnumerator ResetAttackOrDodge(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        OnAttackOrDodgeEnd();
    }

    // This method should be called by animation events at the end of attack or dodge animations
    public void OnAttackOrDodgeEnd()
    {
        isAttackingOrDodging = false;
    }
}