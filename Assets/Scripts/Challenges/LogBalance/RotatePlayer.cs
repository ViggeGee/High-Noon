using UnityEngine;

public class RotatePlayer : MonoBehaviour
{
   [SerializeField] private Transform parentLog;

    public void Update()
    {

        float horizontalInput = 0;
        if (Input.GetKey(KeyCode.A)) horizontalInput = 1;  // Rotate left (counterclockwise)
        if (Input.GetKey(KeyCode.D)) horizontalInput = -1; // Rotate right (clockwise)

        // Rotate around Object A
        transform.RotateAround(parentLog.position, parentLog.transform.right, horizontalInput * 100 * Time.deltaTime);

    }
}
