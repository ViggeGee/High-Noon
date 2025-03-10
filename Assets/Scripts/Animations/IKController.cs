using UnityEngine;
using System.Collections;

public class IKController : MonoBehaviour
{
    private Animator animator;
    public Transform leftHandTarget;
    public Transform[] rightHandTargets;
    public Transform leftFootTarget;
    public Transform rightFootTarget;
    public enum HandState { Holster, Aiming, Recoil }
    public HandState currentHandState = HandState.Holster;
    public float recoilDuration = 0.2f;
    private bool isRecoiling = false;
    public int ammocount = 0;
    public int maxAmmo = 6;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(GameManager.Instance.readyToShoot == true)
        {
            if (Input.GetMouseButtonDown(0) && !isRecoiling && ammocount < maxAmmo)
            {
                NextHandState();
                ammocount++;
            }
        }
      
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (leftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }
            //if(GameManager.Instance.readyToShoot == true)
            {
                if (rightHandTargets != null && rightHandTargets.Length >= 3)
                {
                    Transform target = null;
                    switch (currentHandState)
                    {
                        case HandState.Holster:
                            target = rightHandTargets[0];
                            break;
                        case HandState.Aiming:
                            target = rightHandTargets[1];
                            break;
                        case HandState.Recoil:
                            target = rightHandTargets[2];
                            break;
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
            
        }
    }




    public void NextHandState()
    {
        if (currentHandState == HandState.Holster)
        {
            currentHandState = HandState.Aiming;
        }
        else if (currentHandState == HandState.Aiming && !isRecoiling)
        {
            currentHandState = HandState.Recoil;
            StartCoroutine(RecoilCoroutine());
        }
    }

    private IEnumerator RecoilCoroutine()
    {
        isRecoiling = true;
        yield return new WaitForSeconds(recoilDuration);
        currentHandState = HandState.Aiming;
        isRecoiling = false;
    }

    public void SetHandState(HandState newState)
    {
        if (currentHandState != HandState.Holster && newState == HandState.Holster)
        {
            return;
        }
        currentHandState = newState;
    }
}
