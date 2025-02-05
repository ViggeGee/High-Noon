using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.UI;
//public class ShooterController : MonoBehaviour
//{
//    [SerializeField] private GameObject typeRacerObject;
//    [SerializeField] private GameObject bulletPrefab;
//    [SerializeField] private Transform bulletSpawnPosition;

//    private typeRacer _typeRacer;
//    public Image crossHair;


//    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
//    [SerializeField] private float normalSensitivity;
//    [SerializeField] private float aimSensitivity;
//    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
//    [SerializeField] private Transform debugTransform;

//    private float lastBulletShot;
//    private float fireRate = 0.5f;


//    private StarterAssetsInputs input;
//    private PlayerCamera playerCamera;

//    private void Awake()
//    {
//        input = GetComponent<StarterAssetsInputs>();
//        _typeRacer = typeRacerObject.GetComponent<typeRacer>();
//        playerCamera = GetComponent<PlayerCamera>();

//    }
//    private void Update()
//    {
//        float crossHairSize = 100 + _typeRacer.nrFailLetters * 15f;
//        crossHair.rectTransform.sizeDelta = new Vector2(crossHairSize, crossHairSize);
//        Vector3 mouseWorldPosition = Vector3.zero;

//        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
//        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
//        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
//        {
//            debugTransform.position = raycastHit.point;
//            mouseWorldPosition = raycastHit.point;
//        }

//        Vector3 worldAimTarget = mouseWorldPosition;
//        worldAimTarget.y = transform.position.y;
//        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

//        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);


//        if (input.shoot && GameManager.Instance.readyToShoot)
//        {
//            Vector3 aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
//            if (Time.time > lastBulletShot + fireRate)
//            {
//                lastBulletShot = Time.time;
//                ShootBullet(aimDir);
//                playerCamera.AddRecoil();
//            }

//        }

//    }
//    private void ShootBullet(Vector3 aimDir)
//    {
//        Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
//        input.shoot = false;
//    }

//}



public class ShooterController : MonoBehaviour
{
    [SerializeField] private GameObject typeRacerObject;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private Image crossHair;

    // Crosshair expand/shrink parameters
    [Header("Crosshair Expand Settings")]
    [SerializeField] private float normalSize = 100f;
    [SerializeField] private float expandedSize = 500f;
    [SerializeField] private float shrinkSpeed = 6f; // how fast it returns to normal
    private int nrBulletsShot = 0;
    private float crosshairExpandValue = 0f;
    // 0 => crosshair at normal size
    // 1 => crosshair at expanded size

    private float lastBulletShot;
    private float fireRate = 0.25f;

    private StarterAssetsInputs input;
    private PlayerCamera playerCamera;
    private typeRacer _typeRacer;

    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private Transform debugTransform;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        playerCamera = GetComponent<PlayerCamera>();
        _typeRacer = typeRacerObject.GetComponent<typeRacer>();
    }

    private void Update()
    {
        if (GameManager.Instance.readyToShoot)
        {
            crossHair.gameObject.SetActive(true);
        }

        // 1. Adjust crosshair size based on typeRacer fails (existing logic)
        float crossHairSize = 100 + _typeRacer.nrFailLetters * 15f;
        // We will combine that with our "expand/shrink" logic below.

        // 2. We always let crosshairExpandValue trend back toward 0
        //    This makes the crosshair shrink back to normal over time.
        crosshairExpandValue = Mathf.Lerp(crosshairExpandValue, 0f, Time.deltaTime * shrinkSpeed);

        // 3. Combine "base size" from typeRacer with our expand factor
        float sizeLerp = Mathf.Lerp(normalSize, expandedSize, crosshairExpandValue);
        crossHair.rectTransform.sizeDelta = new Vector2(crossHairSize * (sizeLerp / normalSize),
                                                        crossHairSize * (sizeLerp / normalSize));
        // Explanation: 
        // - crossHairSize is your dynamic base from fails.
        // - sizeLerp is the normal→expanded ratio.
        // - (sizeLerp / normalSize) is effectively a multiplier to keep the base offset 
        //   from fails consistent while also "pulsing" the crosshair.

        // -- Rest of your aiming logic below... --
        //AimAndShoot();

        //float crossHairSize = 100 + _typeRacer.nrFailLetters * 15f;
        //crossHair.rectTransform.sizeDelta = new Vector2(crossHairSize, crossHairSize);
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);


        if (input.shoot && GameManager.Instance.readyToShoot)
        {
            Vector3 aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
            if (Time.time > lastBulletShot + fireRate && nrBulletsShot < 6)
            {
                nrBulletsShot += 1;
                lastBulletShot = Time.time;
                ShootBullet(aimDir);
                playerCamera.AddRecoil();
                crosshairExpandValue = 1f;
            }

        }
    }
    private void ShootBullet(Vector3 aimDir)
    {
        Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        input.shoot = false;
    }

    //private void AimAndShoot()
    //{
    //    Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    //    Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
    //    if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
    //    {
    //        debugTransform.position = raycastHit.point;

    //        Vector3 mouseWorldPosition = raycastHit.point;
    //        Vector3 aimDirection = (mouseWorldPosition - transform.position).normalized;
    //        aimDirection.y = 0f; // if you want to flatten the aim on the Y axis
    //        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

    //        // Check if we can shoot
    //        if (input.shoot && GameManager.Instance.readyToShoot)
    //        {
    //            if (Time.time > lastBulletShot + fireRate)
    //            {
    //                lastBulletShot = Time.time;
    //                ShootBullet(aimDirection);

    //                // Recoil
    //                playerCamera.AddRecoil();

    //                // Here is where we trigger the crosshair to expand:
    //                crosshairExpandValue = 1f;
    //            }
    //        }
    //    }
    //}

    //private void ShootBullet(Vector3 aimDir)
    //{
    //    Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
    //    input.shoot = false;
    //}
}