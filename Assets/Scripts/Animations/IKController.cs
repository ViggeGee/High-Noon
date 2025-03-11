using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class IKController : NetworkBehaviour
{
    private Animator animator;
    public Transform leftHandTarget;
    public Transform[] rightHandTargets;
    public Transform leftFootTarget;
    public Transform rightFootTarget;

    public enum HandState { Holster, Aiming, Recoil }

    public NetworkVariable<HandState> currentHandState = new NetworkVariable<HandState>(HandState.Holster, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> ammocount = new NetworkVariable<int>(0);

    public float recoilDuration = 0.15f;
    private NetworkVariable<bool> isRecoiling = new NetworkVariable<bool>(false);

    public int maxAmmo = 6;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return; 

        if (GameManager.Instance.readyToShoot)
        {
            if (Input.GetMouseButtonDown(0) && !isRecoiling.Value && ammocount.Value < maxAmmo)
            {
                RequestNextHandStateServerRpc();
            }
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!animator) return;

        if (leftHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }

        if (rightHandTargets != null && rightHandTargets.Length >= 3)
        {
            Transform target = null;
            switch (currentHandState.Value)
            {
                case HandState.Holster: target = rightHandTargets[0]; break;
                case HandState.Aiming: target = rightHandTargets[1]; break;
                case HandState.Recoil: target = rightHandTargets[2]; break;
            }

            if (target != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, target.rotation);
            }
        }

        if (leftFootTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
        }

        if (rightFootTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
        }
    }

    [ServerRpc]
    public void RequestNextHandStateServerRpc(ServerRpcParams rpcParams = default)
    {
        if (currentHandState.Value == HandState.Holster)
        {
            currentHandState.Value = HandState.Aiming;
        }
        else if (currentHandState.Value == HandState.Aiming && !isRecoiling.Value)
        {
            currentHandState.Value = HandState.Recoil;
            StartCoroutine(RecoilCoroutine());
        }

        ammocount.Value++;
    }

    private IEnumerator RecoilCoroutine()
    {
        isRecoiling.Value = true;
        yield return new WaitForSeconds(recoilDuration);
        currentHandState.Value = HandState.Aiming;
        isRecoiling.Value = false;
    }

    public void SetHandState(HandState newState)
    {
        if (IsServer)
        {
            if (currentHandState.Value != HandState.Holster && newState == HandState.Holster)
                return;

            currentHandState.Value = newState;
        }
    }
}
