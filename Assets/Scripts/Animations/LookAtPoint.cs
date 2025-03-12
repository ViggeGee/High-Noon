using UnityEngine;
using Unity.Netcode;

public class LookAtPoint : NetworkBehaviour
{
    public Transform target;
    public Transform activetarget;
    public Transform dummyTarget;
    public float rotationSpeed = 5f;
    public float maxYaw = 90f;
    public float maxPitch = 20f;

    private Quaternion initialRotation;
    private NetworkVariable<Vector3> networkTargetPos = new NetworkVariable<Vector3>();

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (target == null || activetarget == null || dummyTarget == null)
            return;

        if (IsServer)
        {
            if (GameManager.Instance.readyToShoot)
                networkTargetPos.Value = target.position;
            else
                networkTargetPos.Value = dummyTarget.position;
        }

        activetarget.position = networkTargetPos.Value;

        Vector3 direction = activetarget.position - transform.position;
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        Quaternion deltaRotation = Quaternion.Inverse(initialRotation) * desiredRotation;
        Vector3 deltaEuler = deltaRotation.eulerAngles;
        deltaEuler.x = Mathf.DeltaAngle(0, deltaEuler.x);
        deltaEuler.y = Mathf.DeltaAngle(0, deltaEuler.y);
        float clampedYaw = Mathf.Clamp(deltaEuler.y, -maxYaw, maxYaw);
        float clampedPitch = Mathf.Clamp(deltaEuler.x, -maxPitch, maxPitch);
        Quaternion clampedDelta = Quaternion.Euler(clampedPitch, clampedYaw, 0);
        Quaternion finalRotation = Quaternion.Slerp(transform.rotation, initialRotation * clampedDelta, rotationSpeed * Time.deltaTime);
        //transform.rotation = finalRotation;
    }
}
