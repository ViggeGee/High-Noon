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
    [SerializeField] private LayerMask aimColliderLayerMask;
    //[SerializeField] private Transform debugTransform;

    private float lastBulletShot;
    private float fireRate = 0.5f;

    private float crossHairExpandValue = 0;
    [Header("Crosshair settings")]
    [SerializeField] private float shrinkSpeed = 6f;
    [SerializeField] private float normalSize = 100f;
    [SerializeField] private float expandedSize = 500f;

    private int numberOfBulletsFired = 0;

    private StarterAssetsInputs input;
    private PlayerCamera playerCamera;

    private const int MAX_NUMBER_OF_BULLETS = 6;
    public override void OnNetworkSpawn()
    {
        if (IsOwner) // Only apply changes for the owning client
        {
            //Debug.Log($"ShooterController spawned for Client ID: {OwnerClientId}");

            // Debugging and safety check
            //Debug.Log($"IsOwner: {IsOwner}, OwnerClientId: {OwnerClientId}");

            if (OwnerClientId == 0)
            {
                characterToActivatePlayer1.SetActive(true);
                //Debug.Log("Activated Player 1 character");
            }
            else
            {
                characterToActivatePlayer2.SetActive(true);
                //Debug.Log("Activated Player 2 character");
            }

            // Set up the input and player camera
            input = GetComponent<StarterAssetsInputs>();
            playerCamera = GetComponent<PlayerCamera>();
            aimVirtualCamera.Priority = 1;

        }
        else
        {
            //Debug.Log($"Player is not owner of shooter controller: {OwnerClientId}");

            // For the non-owner, ensure they can see both players' characters
            if (OwnerClientId == 0 )
            {
                characterToActivatePlayer1.SetActive(true); // Make sure Player 1 is visible for Player 2
                //Debug.Log("Player 2 sees Player 1 character");
            }
            else
            {
                characterToActivatePlayer2.SetActive(true); // Make sure Player 1 is visible for Player 2
                //Debug.Log("Player 1 sees Player 2 character");
            }

            aimVirtualCamera.Priority = 0;
        }
    }


    private void Update()
    {
        if (!IsOwner || !GameManager.Instance.readyToShoot) return; 

        if(GameManager.Instance.readyToShoot)
        {
            crossHair.gameObject.SetActive(true);
        }

        float crossHairSize = 100 + ChallengeManager.Instance.mistakesDuringChallenge;
        crossHairExpandValue = Mathf.Lerp(crossHairExpandValue, 0f, Time.deltaTime * shrinkSpeed);

        float sizeLerp = Mathf.Lerp(normalSize, expandedSize, crossHairExpandValue);
        crossHair.rectTransform.sizeDelta = new Vector2(crossHairSize * (sizeLerp / normalSize), crossHairSize * (sizeLerp / normalSize));

        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask, QueryTriggerInteraction.Collide))
        {
            mouseWorldPosition = raycastHit.point;
            //debugTransform.position = raycastHit.point;
        }

        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

        if (input.shoot && GameManager.Instance.readyToShoot)
        {
            input.shoot = false;
            Vector3 aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;

            if(numberOfBulletsFired >= MAX_NUMBER_OF_BULLETS)
            {
                PlayerOutOfBulletsServerRpc(NetworkManager.Singleton.LocalClientId);
            }

            if (Time.time > lastBulletShot + fireRate && numberOfBulletsFired < MAX_NUMBER_OF_BULLETS)
            {
                numberOfBulletsFired++;
                lastBulletShot = Time.time;

                ShootBulletServerRpc(bulletSpawnPosition.position, aimDir);

                playerCamera.AddRecoil();
                crossHairExpandValue = 1f;
            }
        }
    }

    [ServerRpc]
    private void PlayerOutOfBulletsServerRpc(ulong clientId)
    {
        if (clientId == 0)
        {
            GameManager.Instance.player1OutOfBullets.Value = true;
        }
        else
        {
            GameManager.Instance.player2OutOfBullets.Value = true;
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
