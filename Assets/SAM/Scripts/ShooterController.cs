using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.UI;
public class ShooterController : MonoBehaviour
{
    [SerializeField] private GameObject typeRacerObject;
    private typeRacer _typeRacer;
    public Image crossHair;

    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

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
        if (input.aim && _typeRacer.readyToShoot)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            crossHair.gameObject.SetActive(true);
            playerCamera.SetSensitivity(aimSensitivity);

        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            playerCamera.SetSensitivity(normalSensitivity);
            crossHair.gameObject.SetActive(false);
        }
    }

}
