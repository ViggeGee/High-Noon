using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

public class Motion_Controller : MonoBehaviour
{
    private Rewired.Player player;

    private void Start()
    {
        player = ReInput.players.GetPlayer(0); // Get Player 1

        if (player == null)
        {
            Debug.LogError("<color=red>Player not found! Ensure Rewired is set up correctly.</color>");
            return;
        }

        Debug.Log("<color=green>Player 1: " + player.name + "</color>");
    }

    void Update()
    {
        foreach (Joystick joystick in player.controllers.Joysticks)
        {
            Debug.Log($"<color=yellow>Detected Controller: {joystick.name}</color>");

            //if (joystick.name.Contains("Joy-Con (L)"))
            //{
            //    HandleSwitchController(joystick, "Left Joy-Con");
            //}
            //else if (joystick.name.Contains("Joy-Con (R)"))
            //{
            //    HandleSwitchController(joystick, "Right Joy-Con");
            //}
            //else if (joystick.name.Contains("Pro Controller"))
            //{
            //    HandleProController(joystick, "Switch Pro Controller");
            //}
            if (joystick.name.Contains("Dualshock 4") || joystick.name.Contains("Wireless Controller"))
            {
                HandleDualShock4Controller(joystick);
            }
            else if (joystick.name.Contains("Sony DualSense"))
            {
                HandleDualSenseController(joystick);
            }
        }
    }

    // --- Handling Specific Controllers ---

    private void HandleSwitchController(Joystick joystick, string controllerType)
    {
        var switchExt = joystick.GetExtension<NintendoSwitchJoyConExtension>();
        if (switchExt != null)
        {
            //Vector3 gyroData = switchExt.GetGyroRotationRate();
            //LogGyroData(controllerType, gyroData);
        }
        else
        {
            Debug.LogWarning($"<color=orange>{controllerType} does not support gyroscope.</color>");
        }
    }

    private void HandleProController(Joystick joystick, string controllerType)
    {
        var proExt = joystick.GetExtension<NintendoSwitchJoyConExtension>();
        if (proExt != null)
        {
            //Vector3 gyroData = proExt;
            //LogGyroData(controllerType, gyroData);
        }
        else
        {
            Debug.LogWarning($"<color=orange>{controllerType} does not support gyroscope.</color>");
        }
    }

    private void HandleDualShock4Controller(Joystick joystick)
    {
        var ds4Ext = joystick.GetExtension<DualShock4Extension>();
        if (ds4Ext != null)
        {
            Vector3 gyroData = ds4Ext.GetGyroscopeValue();
            LogGyroData("DualShock 4", gyroData);
        }
    }

    private void HandleDualSenseController(Joystick joystick)
    {
        var ds5Ext = joystick.GetExtension<DualSenseExtension>();
        if (ds5Ext != null)
        {
            Vector3 gyroData = ds5Ext.GetGyroscopeValue();
            LogGyroData("DualSense", gyroData);
        }
    }

    private void LogGyroData(string controllerType, Vector3 gyroData)
    {
        gameObject.transform.Rotate(gyroData * Time.deltaTime * 10);
        Debug.Log($"<color=cyan>{controllerType} Gyro: {gyroData}</color>");
    }
}
