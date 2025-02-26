using UnityEngine;

public class HeadLookAt : MonoBehaviour
{
    private Animator animator;
    public Transform lookAtTarget;
    public float weight = 1f;
    public float bodyWeight = 0f;
    public float headWeight = 1f;
    public float eyesWeight = 1f;
    public float clampWeight = 0.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (lookAtTarget)
        {
            animator.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
            Vector3 pos = lookAtTarget.position;
            pos.y = Mathf.Clamp(pos.y, 0f, 5f);
            animator.SetLookAtPosition(pos);
        }
    }
}
