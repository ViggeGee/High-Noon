using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.UI;
public class ShooterController : MonoBehaviour
{
    [SerializeField] private GameObject typeRacerObject;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPosition;

    private typeRacer _typeRacer;
    public Image crossHair;


    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;


    private StarterAssetsInputs input;
    private PlayerCamera playerCamera;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        _typeRacer = typeRacerObject.GetComponent<typeRacer>();
        playerCamera = GetComponent<PlayerCamera>();
      
    }
    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        //När du håller in aim
        if (input.aim && _typeRacer.readyToShoot)
        {
            if (input.shoot)
            {
                Vector3 aimDir = (debugTransform.position - bulletSpawnPosition.position).normalized;
                ShootBullet(aimDir);
            }


            aimVirtualCamera.gameObject.SetActive(true);
            crossHair.gameObject.SetActive(true);
            playerCamera.SetSensitivity(aimSensitivity);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            playerCamera.SetSensitivity(normalSensitivity);
            crossHair.gameObject.SetActive(false);
        }





    }
    private void ShootBullet(Vector3 aimDir)
    {
        //GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.identity);
        //Rigidbody rb = bullet.GetComponent<Rigidbody>();

        //if (rb != null)
        //{
        //    rb.AddForce(aimDir * 200, ForceMode.Impulse);
        //}
        //Vector3 aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
        Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        input.shoot = false;
    }

}
