using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerLogSlip : NetworkBehaviour
{
    private Transform parentLog;
    private Transform log;
    private int rotationSpeed = 75;

    private Quaternion previousLogRotation;

    private void Start()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(Scenes.LogBalanceScene.ToString())) return;
        parentLog = GameObject.FindGameObjectWithTag("LogBase").transform;

        log = GameObject.FindGameObjectWithTag("Log").transform;

        if (IsServer)
        {
            transform.SetParent(parentLog);
        }

        previousLogRotation = log.rotation;
    }
    public void Update()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(Scenes.LogBalanceScene.ToString())) return;

       // if (!GameManager.Instance.hasGameStarted.Value || !GameManager.Instance.isPlayer1Ready.Value || !GameManager.Instance.isPlayer2Ready.Value || GameManager.Instance.playerDied.Value) return;

        CheckTiltAngle();

        RotatePlayerWKeyBoard();

        FollowLogRotation();
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
        if(NetworkManager.LocalClientId == 0)
        {
            transform.RotateAround(log.position, -log.transform.right, horizontalInput * rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(log.position, log.transform.right, horizontalInput * rotationSpeed * Time.deltaTime);
        }
       
    }
    private void FollowLogRotation()
    {
        Vector3 position = transform.position;

        // Calculate the difference in rotation since last frame
        Quaternion logRotationDelta = log.rotation * Quaternion.Inverse(previousLogRotation);

        // Apply the same rotation difference to the player
        transform.rotation = logRotationDelta * transform.rotation;

        // Move the player around the log to maintain position
        transform.RotateAround(log.position, log.up, logRotationDelta.eulerAngles.y);
        transform.position = position;

        // Update the stored previous rotation
        previousLogRotation = log.rotation;
    }
}
