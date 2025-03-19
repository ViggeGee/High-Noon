using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerLogSlip : NetworkBehaviour
{
   private Transform parentLog;
    private int rotationSpeed = 75;

    private void Start()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(Scenes.LogBalanceScene.ToString())) return;
        parentLog = GameObject.FindGameObjectWithTag("Log").transform;

        if(IsServer)
        {
            transform.SetParent(parentLog);
        }
        
    }
    public void Update()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(Scenes.LogBalanceScene.ToString())) return;

        CheckTiltAngle();

        RotatePlayerWKeyBoard();

    }
    void CheckTiltAngle()//If player is tilted over a certain angle gravity gets activated and the player falls in the water
    {
        float angle = Vector3.Angle(transform.up, Vector3.up); // Get angle between child’s up and world up

        if (angle >= 55f) // Check if angle is close to 20 degrees
        {
            Player player = GetComponent<Player>();
            player.TakeDamage(Player.DamageType.Head);
        }
        
    }
    public void RotatePlayerWKeyBoard()//rotate player with keyboard , was used for testing
    {

        float horizontalInput = 0;
        if (Input.GetKey(KeyCode.A)) horizontalInput = 1;  // Rotate left (counterclockwise)
        if (Input.GetKey(KeyCode.D)) horizontalInput = -1; // Rotate right (clockwise)

        // Rotate around parentLog
        transform.RotateAround(parentLog.position, parentLog.transform.right, horizontalInput * rotationSpeed * Time.deltaTime);
    }
}
