using UnityEngine;
using Unity.Netcode;

public class HeadLookAt : NetworkBehaviour
{
    private Animator animator;
    public Transform lookAtTarget;
    public float weight = 1f;
    public float bodyWeight = 0f;
    public float headWeight = 1f;
    public float eyesWeight = 1f;
    public float clampWeight = 0.5f;

    private NetworkVariable<Vector3> networkLookAtPos = new NetworkVariable<Vector3>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (lookAtTarget == null) return;

        if (IsOwner)
        {
            RequestLookAtPositionServerRpc(lookAtTarget.position);
        }

       
        animator.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        animator.SetLookAtPosition(networkLookAtPos.Value);
    }

    [ServerRpc]
    private void RequestLookAtPositionServerRpc(Vector3 newPosition)
    {
        networkLookAtPos.Value = newPosition;
    }
}
