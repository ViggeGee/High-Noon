using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.UI;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
public class ShooterController : NetworkBehaviour
{
    //[SerializeField] private GameObject typeRacerObject;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private GameObject characterToActivatePlayer1;
    [SerializeField] private GameObject characterToActivatePlayer2;
     
    public Image crossHair;


    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    //[SerializeField] private Transform debugTransform;

    private float lastBulletShot;
    private float fireRate = 0.5f;


    private StarterAssetsInputs input;
    private PlayerCamera playerCamera;

    public override void OnNetworkSpawn()
    {
        if (IsOwner) // Only apply changes for the owning client
        {
            Debug.Log($"ShooterController spawned for Client ID: {OwnerClientId}");

            // Debugging and safety check
            Debug.Log($"IsOwner: {IsOwner}, OwnerClientId: {OwnerClientId}");

            if (OwnerClientId == 0)
            {
                characterToActivatePlayer1.SetActive(true);
                Debug.Log("Activated Player 1 character");
            }
            else if (OwnerClientId == 1)
            {
                characterToActivatePlayer2.SetActive(true);
                Debug.Log("Activated Player 2 character");
            }

            // Set up the input and player camera
            input = GetComponent<StarterAssetsInputs>();
            playerCamera = GetComponent<PlayerCamera>();
            aimVirtualCamera.Priority = 1;

            // Ensure the player controls only their own character
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.SpawnAsPlayerObject(OwnerClientId); // Ensure correct ownership
            }
        }
        else
        {
            Debug.Log($"Player is not owner of shooter controller: {OwnerClientId}");

            // For the non-owner, ensure they can see both players' characters
            if (OwnerClientId == 0 )
            {
                characterToActivatePlayer1.SetActive(true); // Make sure Player 1 is visible for Player 2
                Debug.Log("Player 2 sees Player 1 character");
            }
            else if (OwnerClientId == 1)
            {
                characterToActivatePlayer2.SetActive(true); // Make sure Player 1 is visible for Player 2
                Debug.Log("Player 1 sees Player 2 character");
            }

            aimVirtualCamera.Priority = 0;
        }
    }




    private void Update()
    {
        if (!IsOwner) return; // Only allow the local player to shoot

        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
        {
            mouseWorldPosition = raycastHit.point;
        }

        if (input.shoot && GameManager.Instance.readyToShoot)
        {
            input.shoot = false;
            Vector3 aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;

            if (Time.time > lastBulletShot + fireRate)
            {
                lastBulletShot = Time.time;

                ShootBulletServerRpc(bulletSpawnPosition.position, aimDir);

                playerCamera.AddRecoil();
            }
        }
    }

    [ServerRpc]
    private void ShootBulletServerRpc(Vector3 spawnPosition, Vector3 aimDir)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(aimDir, Vector3.up));
        NetworkObject bulletNetObj = bullet.GetComponent<NetworkObject>();

        if (bulletNetObj != null)
        {
            bulletNetObj.Spawn(); // Server owns the bullet, syncs to all clients
            bullet.GetComponent<Rigidbody>().AddForce(aimDir * 200, ForceMode.Impulse);
        }
    }


}
