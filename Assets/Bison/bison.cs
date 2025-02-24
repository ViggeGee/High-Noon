using System.Collections;
using UnityEngine;

public class bison : MonoBehaviour
{
    private Animator animator;
    private Animation animationComponent;

    void Start()
    {
        animator = GetComponent<Animator>();
        animationComponent = GetComponent<Animation>();

        if (animator != null)
        {
            RandomizeAnimator();
        }
        else if (animationComponent != null)
        {
            RandomizeLegacyAnimation();
        }
        else
        {
            Debug.LogWarning("No Animator or Animation component found on " + gameObject.name);
        }
        StartCoroutine(SelfDestruct());
    }

    void RandomizeAnimator()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float randomStart = Random.Range(0f, 1f);
        animator.Play(stateInfo.fullPathHash, 0, randomStart);
    }

    void RandomizeLegacyAnimation()
    {
        foreach (AnimationState animState in animationComponent)
        {
            animState.time = Random.Range(0f, animState.length);
            animationComponent.Play();
        }
    }

    public IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
