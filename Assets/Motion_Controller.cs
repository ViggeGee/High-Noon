using UnityEngine;
public class Motion_Controller : MonoBehaviour
{
    private int controllerIndex = 0; // The controller to check, default is 0

    void Update()
    {
        // Get the joystick from the input system
        if (Input.GetJoystickNames().Length > controllerIndex)
        {
            string joystickName = Input.GetJoystickNames()[controllerIndex];

            // Check if the joystick supports gyroscope data (typically PlayStation or Switch controllers)
            if (joystickName.Contains("Wireless Controller") || joystickName.Contains("Pro Controller") || joystickName.Contains("8BitDo Receiver"))
            {
                // Retrieve gyroscopic data from the joystick
                Vector3 gyroData = Input.gyro.rotationRate;

                // Output the gyroscopic data (rotation in degrees per second)
                Debug.Log("Gyroscope data: " + gyroData);
            }
        }
    }
}
