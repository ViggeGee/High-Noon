using Cinemachine;
using UnityEngine;

public class FindCameraFollowTarget : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private GameObject followTarget;
    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        followTarget = GameObject.FindGameObjectWithTag("CinemachineTarget");

        if(followTarget != null && virtualCamera != null)
        {
            virtualCamera.Follow = followTarget.transform;
        }
        
    }
    private void Update()
    {
        if(followTarget == null || virtualCamera == null && GameManager.playersJoined > 0)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            followTarget = GameObject.FindGameObjectWithTag("CinemachineTarget");

            if(followTarget != null && virtualCamera != null)
            {
                virtualCamera.Follow = followTarget.transform;
            }
        }
            
    }

}
