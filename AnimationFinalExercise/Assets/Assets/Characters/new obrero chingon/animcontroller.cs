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
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetTrigger("DodgeLowAttack");
        }

        if (Input.GetKeyDown(KeyCode.H) && !isPerformingBigStep)
        {
            StartCoroutine(PerformBigStep());
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            animator.SetTrigger("LowQuickAttack");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            animator.SetTrigger("LowSlowAttack");
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

            attackIndex = (attackIndex + 1) % 4; // Cycle through the sequence
        }
    }
}
