using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class LookAtPoint : NetworkBehaviour
{
    public Transform defaultTarget;
    public float rotationSpeed = 5f;
    public float maxYaw = 90f;
    public float maxPitch = 20f;

    private Quaternion initialRotation;
    private Vector3 smoothTargetPos; 

    private NetworkVariable<Vector3> networkTargetPos = new NetworkVariable<Vector3>(
        Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    private Transform cameraTarget;

    void Start()
    {
        initialRotation = transform.rotation;
        FindCameraTarget();
    }

    void LateUpdate()
    {
        if (cameraTarget == null || defaultTarget == null)
        {
            FindCameraTarget();
            return;
        }

        if (IsOwner) 
        {
            Vector3 newTargetPos = cameraTarget.position; 
            UpdateTargetPositionServerRpc(newTargetPos);
        }


        smoothTargetPos = Vector3.Lerp(smoothTargetPos, networkTargetPos.Value, Time.deltaTime * 5f);

  
        Vector3 lookTargetPos = (smoothTargetPos != Vector3.zero) ? smoothTargetPos : defaultTarget.position;

        Vector3 direction = lookTargetPos - transform.position;
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        Quaternion deltaRotation = Quaternion.Inverse(initialRotation) * desiredRotation;
        Vector3 deltaEuler = deltaRotation.eulerAngles;
        deltaEuler.x = Mathf.DeltaAngle(0, deltaEuler.x);
        deltaEuler.y = Mathf.DeltaAngle(0, deltaEuler.y);
        float clampedYaw = Mathf.Clamp(deltaEuler.y, -maxYaw, maxYaw);
        float clampedPitch = Mathf.Clamp(deltaEuler.x, -maxPitch, maxPitch);
        Quaternion clampedDelta = Quaternion.Euler(clampedPitch, clampedYaw, 0);
        Quaternion finalRotation = Quaternion.Slerp(transform.rotation, initialRotation * clampedDelta, rotationSpeed * Time.deltaTime);
        transform.rotation = finalRotation;
    }


    private void FindCameraTarget()
    {
        CinemachineBrain brain = Camera.main?.GetComponent<CinemachineBrain>();
        if (brain != null && brain.ActiveVirtualCamera != null)
        {
            CinemachineVirtualCamera vCam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
            if (vCam != null)
            {
                cameraTarget = vCam.transform; 
            }
        }
    }

    [ServerRpc]
    private void UpdateTargetPositionServerRpc(Vector3 newPosition)
    {
        networkTargetPos.Value = newPosition;
        UpdateTargetPositionClientRpc(newPosition);
    }

    [ClientRpc]
    private void UpdateTargetPositionClientRpc(Vector3 newPosition)
    {
        networkTargetPos.Value = newPosition;
    }
}
