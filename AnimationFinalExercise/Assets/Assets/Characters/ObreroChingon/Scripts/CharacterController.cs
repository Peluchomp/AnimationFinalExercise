using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    public float speed = 5f;  // Movement speed

    private float idleTimer = 0f; // Timer to track idle duration
    private bool isIdling = false; // Is the character idling?
    private bool isAttacking = false; // Is the character currently attacking?
    private int punchComboState = 0; // Tracks the current punch combo state
    private float comboResetTimer = 0f; // Timer to reset the combo
    public float comboResetTime = 1.0f; // Time to reset combo if no input is received

    private bool isCrouching = false; // Is the character crouching?
    private bool isDead = false; // Is the character dead?

    void Update()
    {
        if (isDead) return; // Disable all actions if the player is dead

        if (!isAttacking && !isCrouching)
        {
            HandleMovement();
            HandleIdleState();
        }

        HandleAttack();
        HandleDodge();
        HandleCrouch();
        HandleSpecialActions();
    }

    void HandleMovement()
    {
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.D)) // Move forward (-z axis)
        {
            moveZ = -1f;
            animator.ResetTrigger("Idle2");
            animator.SetTrigger("WalkForward");
            idleTimer = 0f; // Reset idle timer
            isIdling = false;
        }
        else if (Input.GetKey(KeyCode.A)) // Move backward (+z axis)
        {
            moveZ = 1f;
            animator.ResetTrigger("Idle2");
            animator.SetTrigger("WalkBackward");
            idleTimer = 0f; // Reset idle timer
            isIdling = false;
        }
        else
        {
            animator.ResetTrigger("WalkForward");
            animator.ResetTrigger("WalkBackward");
            isIdling = true;
        }

        // Move the character
        Vector3 move = new Vector3(0, 0, moveZ) * speed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    void HandleIdleState()
    {
        if (isIdling)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= 15f)
            {
                animator.SetTrigger("Idle2"); // Transition to Idle 2
                idleTimer = 0f; // Reset timer after switching to Idle 2
            }
        }
        else
        {
            idleTimer = 0f; // Reset timer if not idling
        }
    }

    void HandleAttack()
    {
        // Handle Kick (Right Click)
        if (Input.GetMouseButtonDown(1)) // Right click for Kick
        {
            if (!isAttacking)
            {
                animator.SetTrigger("Kick");
                StartCoroutine(AttackCooldown(0.8f)); // Example cooldown for kick
            }
        }

        // Handle Punch Combo (Left Click)
        if (Input.GetMouseButtonDown(0)) // Left click for Punch
        {
            if (!isAttacking)
            {
                if (punchComboState == 0)
                {
                    animator.SetTrigger("Punch");
                }
                else if (punchComboState == 1)
                {
                    animator.SetTrigger("Punch2");
                }
                else if (punchComboState == 2)
                {
                    animator.SetTrigger("Punch3");
                }

                punchComboState = (punchComboState + 1) % 3; // Cycle through 0, 1, 2
                comboResetTimer = 0f; // Reset the combo reset timer

                StartCoroutine(AttackCooldown(0.5f)); // Cooldown between punches
            }
        }

        // Reset Punch Combo if no input for a certain time
        if (punchComboState > 0)
        {
            comboResetTimer += Time.deltaTime;
            if (comboResetTimer >= comboResetTime)
            {
                punchComboState = 0; // Reset combo
                comboResetTimer = 0f;
            }
        }
    }

    void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && !isCrouching)
        {
            animator.SetTrigger("Dodge");
            StartCoroutine(AttackCooldown(0.8f)); // Dodge duration (disables movement temporarily)
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isCrouching = true;
            animator.SetTrigger("Crouch");
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            isCrouching = false;
            animator.ResetTrigger("Crouch");
        }
    }

    void HandleSpecialActions()
    {
        // Low Attack (Q)
        if (Input.GetKeyDown(KeyCode.Q) && !isAttacking)
        {
            animator.SetTrigger("LowAttack");
            StartCoroutine(AttackCooldown(0.5f));
        }

        // Heavy Low Attack (Hold Q)
        if (Input.GetKey(KeyCode.Q) && !isAttacking)
        {
            animator.SetTrigger("HeavyLowAttack");
            StartCoroutine(AttackCooldown(1.0f));
        }

        // Body Hit (F1)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            animator.SetTrigger("BodyHit");
        }

        // Head Hit (F2)
        if (Input.GetKeyDown(KeyCode.F2))
        {
            animator.SetTrigger("HeadHit");
        }

        // Death (F3)
        if (Input.GetKeyDown(KeyCode.F3))
        {
            animator.SetTrigger("Death");
            StartCoroutine(HandleDeath());
        }
    }

    IEnumerator HandleDeath()
    {
        isDead = true;
        yield return new WaitForSeconds(0.1f); // Short delay to ensure death animation starts
        animator.SetBool("IsDead", true); // Set a parameter to lock the character in the death state
    }

    IEnumerator AttackCooldown(float duration)
    {
        isAttacking = true;
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }
}
