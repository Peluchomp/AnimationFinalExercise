using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    private Animator mAnimator;
    public float moveDistance = 10f; // Distance to move forward or backward per animation loop
    private Coroutine moveCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            bool isWalkingForward = Input.GetKey(KeyCode.D);
            bool isWalkingBackward = Input.GetKey(KeyCode.A);

            mAnimator.SetBool("IsWalkingForward", isWalkingForward);
            mAnimator.SetBool("IsWalkingBackward", isWalkingBackward);

            if (isWalkingForward && moveCoroutine == null)
            {
                moveCoroutine = StartCoroutine(GradualMove(Vector3.forward));
            }
            else if (isWalkingBackward && moveCoroutine == null)
            {
                moveCoroutine = StartCoroutine(GradualMove(Vector3.back));
            }
            else if (!isWalkingForward && !isWalkingBackward && moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
        }
    }

    private IEnumerator GradualMove(Vector3 direction)
    {
        while (true)
        {
            AnimatorStateInfo stateInfo = mAnimator.GetCurrentAnimatorStateInfo(0);
            float animationDuration = stateInfo.length;
            float moveStep = moveDistance / animationDuration;
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                transform.position -= direction * moveStep * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
