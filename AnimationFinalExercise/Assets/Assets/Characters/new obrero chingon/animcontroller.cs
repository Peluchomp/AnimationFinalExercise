using System.Collections;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;

    private int attackIndex = 0;
    private float lastAttackTime;
    private float attackResetTime = 2f; // Time after which the attack sequence resets

    public float movementSpeed = 2f; // Speed of character movement
    public float bigStepDistance = 3f; // Distance covered during a big step forward
    private bool isPerformingBigStep = false;
    private bool isAttackingOrDodging = false; // Flag to check if the player is attacking or dodging

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

        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("WalkingBackward", true);
            moveDirection = -1f; // Move backward
        }
        else
        {
            animator.SetBool("WalkingBackward", false);
        }

        if (Input.GetKey(KeyCode.D))
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

        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetTrigger("DodgeHighAttack");
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetTrigger("DodgeLowAttack");
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));
        }

        if (Input.GetKeyDown(KeyCode.H) && !isPerformingBigStep)
        {
            StartCoroutine(PerformBigStep());
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            animator.SetTrigger("LowQuickAttack");
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            animator.SetTrigger("LowSlowAttack");
            isAttackingOrDodging = true;
            StartCoroutine(ResetAttackOrDodge(animator.GetCurrentAnimatorStateInfo(0).length));
        }
    }

    private System.Collections.IEnumerator PerformBigStep()
    {
        yield return new WaitForSeconds(0.2f); // Wait for 0.2 seconds before starting movement
        isPerformingBigStep = true;
        animator.SetTrigger("SlowAttackLongRange");

        float elapsedTime = 0f;
        float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length; // Get the duration of the animation

        while (elapsedTime < animationDuration)
        {
            transform.Translate(Vector3.forward * (bigStepDistance / animationDuration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isPerformingBigStep = false;
    }

    void HandleAttacks()
    {
        if (Time.time - lastAttackTime > attackResetTime)
        {
            attackIndex = 0; // Reset the attack sequence after 2 seconds
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            lastAttackTime = Time.time; // Update last attack time

            switch (attackIndex)
            {
                case 0:
                    animator.SetTrigger("Jab");
                    break;
                case 1:
                    animator.SetTrigger("Hook");
                    break;
                case 2:
                    animator.SetTrigger("Knee");
                    break;
                case 3:
                    animator.SetTrigger("Jab");
                    break;
            }

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