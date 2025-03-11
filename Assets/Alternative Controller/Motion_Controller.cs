using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;
using System.Collections.Generic;

public class Motion_Controller : MonoBehaviour
{
    private Rewired.Player player1;
    public GameObject player1GameObject;
    public GameObject player1PivotAroundGameObject;

    public bool twoPlayers;

    private Rewired.Player player2;
    public GameObject player2GameObject;
    public GameObject player2PivotAroundGameObject;

    private List<Rewired.Player> players = new List<Rewired.Player>();

    public float rotationAmount = 20;
    public float angle = 10;


    private void Start()
    {
        player1 = ReInput.players.GetPlayer(0);
        if (twoPlayers)
            player2 = ReInput.players.GetPlayer(1);

        if (player1 == null)
        {
            Debug.LogError("<color=red>Player 1 not found!</color>");
            return;
        }
        else if (twoPlayers == true && player2 == null)
        {
            Debug.LogError("<color=red>Player 2 not found!</color>");
            return;
        }
        else
        {
            players.Add(player1);

            if (twoPlayers)
                players.Add(player2);
        }

        Debug.Log("<color=green>Player 1: " + player1.name + "</color>");
        if (twoPlayers)
            Debug.Log("<color=green>Player 2: " + player2.name + "</color>");
    }

    void Update()
    {
        foreach (Rewired.Player player in players)
        {
            UseControllers(player);
        }
    }

    #region Not Working - Switch Controllers
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
    #endregion

    private Vector3 HandleDualShock4Controller(Joystick joystick)
    {
        var ds4Ext = joystick.GetExtension<DualShock4Extension>();
        if (ds4Ext != null)
        {
            Vector3 gyroData = ds4Ext.GetGyroscopeValue();
            return gyroData;
        }
        else
        {
            Debug.LogWarning("<color=orange>DualShock 4 value is null.</color>");
            return Vector3.zero;
        }
    }

    private Vector3 HandleDualSenseController(Joystick joystick)
    {
        var ds5Ext = joystick.GetExtension<DualSenseExtension>();
        if (ds5Ext != null)
        {
            Vector3 gyroData = ds5Ext.GetGyroscopeValue();
            return gyroData;
        }
        else
        {
            Debug.LogWarning("<color=orange>DualSense value is null.</color>");
            return Vector3.zero;
        }
    }


    public void UseControllers(Rewired.Player player)
    {
        foreach (Joystick joystick in player.controllers.Joysticks)
        {
            Debug.Log($"<color=yellow>Player: {player.name} Detected Controller: {joystick.name}</color>");

            #region Not Working - Switch Controllers
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
            #endregion

            if (joystick.name.Contains("Dualshock 4") || joystick.name.Contains("Wireless Controller"))
            {
                Vector3 gyroData = HandleDualShock4Controller(joystick);
                PivotObjectWithController("Dualshock 4", player, gyroData);
            }
            else if (joystick.name.Contains("Sony DualSense"))
            {
                Vector3 gyroData = HandleDualSenseController(joystick);
                PivotObjectWithController("DualSense", player, gyroData);
            }
        }
    }

    private void PivotObjectWithController(string controllerType, Rewired.Player player, Vector3 gyroData)
    {
        if (gyroData.sqrMagnitude < 0.01f) return; // Ignore small gyro movements

        GameObject playerGameObject = GetPlayerGameObject(player);
        GameObject pivotAroundGameObject = GetPlayerPivotGameObject(player);

        if (playerGameObject == null || pivotAroundGameObject == null)
        {
            Debug.LogWarning("Pivot or player GameObject is null.");
            return;
        }

        Vector3 pivotPosition = pivotAroundGameObject.transform.position;

        // 1️⃣ **Use X-axis for side-to-side balance effect**
        Vector3 rotationAxis = pivotAroundGameObject.transform.right; // Ensures a proper balancing pivot
        float rotationSpeed = gyroData.z * rotationAmount * Time.deltaTime; // Z gyro controls X-axis tilt

        // 2️⃣ **Rotate character around the log’s X-axis to simulate balance**
        playerGameObject.transform.RotateAround(pivotPosition, rotationAxis, rotationSpeed);

        Debug.Log($"Pivoting properly! Speed: {rotationSpeed}, Axis: {rotationAxis}");
    }


    private void RotateObjectWithController(string controllerType, Rewired.Player player, Vector3 gyroData)
    {
        //gameObject.transform.Rotate(gyroData * Time.deltaTime * 10);

        Vector3 gyroDataX = new Vector3(gyroData.x, 0, 0);
        Vector3 gyroDataY = new Vector3(0, gyroData.y, 0);
        Vector3 gyroDataZ = new Vector3(0, 0, gyroData.z);

        //gameObject.transform.Rotate(gyroDataX * Time.deltaTime * rotationAmount);
        //gameObject.transform.Rotate(gyroDataY * Time.deltaTime * rotationAmount);
        player1GameObject.transform.Rotate(gyroDataZ * Time.deltaTime * rotationAmount);

        Debug.Log($"<color=cyan>{controllerType} Gyro: {gyroData}</color>");
    }

    public GameObject GetPlayerGameObject(Rewired.Player player)
    {
        if (player == player1)
        {
            return player1GameObject;
        }
        else if (twoPlayers == true && player == player2)
        {
            return player2GameObject;
        }
        else
        {
            return null;
        }
    }
    public GameObject GetPlayerPivotGameObject(Rewired.Player player)
    {
        if (player == player1)
        {
            return player1PivotAroundGameObject;
        }
        else if (twoPlayers == true && player == player2)
        {
            return player2PivotAroundGameObject;
        }
        else
        {
            return null;
        }
    }
}
