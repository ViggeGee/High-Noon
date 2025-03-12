using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Cinemachine;

public class IKController : NetworkBehaviour
{
    private Animator animator;
    public Transform leftHandTarget;
    public Transform[] rightHandTargets;
    public Transform leftFootTarget;
    public Transform rightFootTarget;
    public GameObject gun;
    
    public enum HandState { Holster, Aiming, Recoil }

    public NetworkVariable<HandState> currentHandState = new NetworkVariable<HandState>(HandState.Holster, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> ammocount = new NetworkVariable<int>(0);

    public float recoilDuration = 0.15f;
    private NetworkVariable<bool> isRecoiling = new NetworkVariable<bool>(false);

    public int maxAmmo = 6;
    private Vector3 velocity = Vector3.zero;
    private Quaternion latestCameraRotation;

    public NetworkVariable<Quaternion> networkCameraRotation = new NetworkVariable<Quaternion>(
      Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> isGunActive = new NetworkVariable<bool>(false); 
    void Start()
    {
        animator = GetComponent<Animator>();
        //isGunActive.OnValueChanged += (oldValue, newValue) =>
        //{
        //    gun.SetActive(newValue);
        //};
    }

    void Update()
    {
        if (!IsOwner) return; 

        if (GameManager.Instance.readyToShoot)
        {
            ToggleGunServerRpc();
            if (Input.GetMouseButtonDown(0) && !isRecoiling.Value && ammocount.Value < maxAmmo)
            {
                RequestNextHandStateServerRpc();
            }
        }

        UpdateCameraRotationServerRpc(GetCameraRotation());
    }
    void ToggleGun()
    {
        gun.SetActive(true); 
    }

    void LateUpdate()
    {
        if (IsOwner)
        {
            latestCameraRotation = GetCameraRotation();
        }
        else
        {
            latestCameraRotation = networkCameraRotation.Value; 
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!animator ) return; 

        Quaternion cameraRotation = latestCameraRotation;
        Vector3 aimDirection = cameraRotation * Vector3.forward;

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
                case HandState.Holster: target = rightHandTargets[0];
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, target.rotation);
                    break;

                case HandState.Aiming: 
                    target = rightHandTargets[1];
             

                    Vector3 newTargetPosition = target.position + aimDirection * 0.5f;
                    Quaternion newTargetRotation = Quaternion.LookRotation(aimDirection) * Quaternion.Euler(0, 0, -80);
                    Vector3 aimUp = latestCameraRotation * Vector3.up;

                    target.position = Vector3.Lerp(target.position, newTargetPosition, Time.deltaTime * 10f);
                    target.rotation = Quaternion.Slerp(target.rotation, newTargetRotation, Time.deltaTime * 10f);

                 
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, target.rotation);

              
                    break;
                case HandState.Recoil: target = rightHandTargets[2];
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, target.rotation);
                    break;
            }
         
            if (target != null)
            {

                //Vector3 newTargetPosition = target.position + aimDirection * 0.5f;
                //Quaternion newTargetRotation = cameraRotation;
                //target.position = Vector3.SmoothDamp(target.position, newTargetPosition, ref velocity, 0.1f);
                //target.rotation = Quaternion.Slerp(target.rotation, newTargetRotation, Time.deltaTime * 10f);


                //animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

                //animator.SetIKPosition(AvatarIKGoal.RightHand, Vector3.Lerp(animator.GetIKPosition(AvatarIKGoal.RightHand), newTargetPosition, Time.deltaTime * 10f));
                //animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.RightHand), newTargetRotation, Time.deltaTime * 10f));



                //animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                //animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
                //animator.SetIKRotation(AvatarIKGoal.RightHand, target.rotation);
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


    private Vector3 GetCameraForward()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain != null && brain.ActiveVirtualCamera != null)
        {
            CinemachineVirtualCamera vCam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
            if (vCam != null)
            {
                return vCam.transform.forward; 
            }
        }

        return Camera.main.transform.forward; 
    }
    private Quaternion GetCameraRotation()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain != null && brain.ActiveVirtualCamera != null)
        {
            CinemachineVirtualCamera vCam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
            if (vCam != null)
            {
                return vCam.transform.rotation;
            }
        }

        return Camera.main.transform.rotation; 
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

    [ServerRpc]
    private void UpdateCameraRotationServerRpc(Quaternion newRotation)
    {
        networkCameraRotation.Value = newRotation;
    }

    [ServerRpc]
    private void ToggleGunServerRpc()
    {
        bool newState = !isGunActive.Value;
        isGunActive.Value = newState;
        ToggleGunClientRpc(true);
    }
    [ClientRpc]
    private void ToggleGunClientRpc(bool newState)
    {
        gun.SetActive(newState);
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
