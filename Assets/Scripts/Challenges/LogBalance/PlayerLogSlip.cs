using UnityEngine;

public class PlayerLogSlip : MonoBehaviour
{
   [SerializeField] private Transform parentLog;
    private int rotationSpeed = 75;
    public void Update()
    {
        CheckTiltAngle();

        float horizontalInput = 0;
        if (Input.GetKey(KeyCode.A)) horizontalInput = 1;  // Rotate left (counterclockwise)
        if (Input.GetKey(KeyCode.D)) horizontalInput = -1; // Rotate right (clockwise)

        // Rotate around parentLog
        transform.RotateAround(parentLog.position, parentLog.transform.right, horizontalInput * rotationSpeed * Time.deltaTime);

    }
    void CheckTiltAngle()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up); // Get angle between child’s up and world up

        if (angle >= 55f) // Check if angle is close to 20 degrees
        {
            GetComponent<Rigidbody>().useGravity = true;
        }
        
    }
}
