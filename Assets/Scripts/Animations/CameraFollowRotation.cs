using Cinemachine;
using UnityEngine;

public class CameraFollowRotation : MonoBehaviour
{
    public Transform target; 
    private CinemachineBrain cinemachineBrain;

    void Start()
    {
   
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
    }

    void Update()
    {
        if (target == null)
        {

            return;
        }


        Quaternion targetRotation = GetCameraRotation();

  
        transform.rotation = targetRotation;
    }

    private Quaternion GetCameraRotation()
    {
    
        if (cinemachineBrain != null && cinemachineBrain.ActiveVirtualCamera != null)
        {
            CinemachineVirtualCamera vCam = cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
            if (vCam != null)
            {
                return vCam.transform.rotation;
            }
        }

        return target.rotation;
    }
}
