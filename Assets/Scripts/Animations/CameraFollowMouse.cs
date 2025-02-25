using UnityEngine;

public class CameraFollowMouse : MonoBehaviour
{

    public float mouseSensitivity = 100f;

 
    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
     
  
    }

    void Update()
    {
       
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        yRotation += mouseX;
        xRotation -= mouseY;


        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
