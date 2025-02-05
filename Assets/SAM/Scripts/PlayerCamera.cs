using StarterAssets;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;
using System.Collections;




//public class PlayerCamera : MonoBehaviour
//{
//    private StarterAssetsInputs _input;
//    private const float _threshold = 0.01f;
//    public bool LockCameraPosition = false;
//    public float sensitivity = 1f;

//    private float _cinemachineTargetYaw;
//    private float _cinemachineTargetPitch;
//    private float recoilOffset = 0f; // New variable to track recoil separately

//    public GameObject CinemachineCameraTarget;

//    [Tooltip("How far in degrees can you move the camera up")]
//    public float TopClamp = 70.0f;

//    [Tooltip("How far in degrees can you move the camera down")]
//    public float BottomClamp = -30.0f;

//    [Tooltip("Additional degrees to override the camera. Useful for fine-tuning camera position when locked")]
//    public float CameraAngleOverride = 0.0f;

//    private PlayerInput _playerInput;

//    private bool IsCurrentDeviceMouse
//    {
//        get
//        {
//#if ENABLE_INPUT_SYSTEM
//            return _playerInput.currentControlScheme == "KeyboardMouse";
//#else
//            return false;
//#endif
//        }
//    }

//    private bool isRecoiling = false;
//    private float recoilAmount = 10f;
//    private float recoilSpeed = 100f;
//    private float recoverySpeed = 50f;

//    void Start()
//    {
//        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
//        _input = GetComponent<StarterAssetsInputs>();
//        _playerInput = GetComponent<PlayerInput>();
//    }

//    void Update()
//    {
//        CameraRotation();
//    }

//    private void CameraRotation()
//    {
//        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
//        {
//            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

//            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * sensitivity;
//            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * sensitivity;
//        }

//        // Clamp the player's input-based pitch
//        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

//        // Apply recoil offset **without overriding player input**
//        float finalPitch = _cinemachineTargetPitch + recoilOffset;

//        // Ensure final pitch stays within bounds
//        finalPitch = ClampAngle(finalPitch, BottomClamp, TopClamp);

//        // Apply rotation to the camera
//        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(finalPitch + CameraAngleOverride,
//            _cinemachineTargetYaw, 0.0f);
//    }

//    private static float ClampAngle(float angle, float min, float max)
//    {
//        if (angle < -360f) angle += 360f;
//        if (angle > 360f) angle -= 360f;
//        return Mathf.Clamp(angle, min, max);
//    }

//    public void SetSensitivity(float newSensitivity)
//    {
//        sensitivity = newSensitivity;
//    }

//    public void AddRecoil()
//    {
//        if (!isRecoiling)
//        {
//            StartCoroutine(ApplyRecoil());
//        }
//    }

//    private IEnumerator ApplyRecoil()
//    {
//        isRecoiling = true;
//        float targetOffset = -recoilAmount; // Negative makes the camera go UP
//        float elapsedTime = 0f;

//        // Move camera up with recoil effect
//        while (elapsedTime < (recoilAmount / recoilSpeed))
//        {
//            recoilOffset = Mathf.Lerp(0, targetOffset, elapsedTime / (recoilAmount / recoilSpeed));
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }
//        recoilOffset = targetOffset;

//        // Wait briefly at the top
//        yield return new WaitForSeconds(0.1f);

//        // Move camera back down (smooth recovery)
//        elapsedTime = 0f;
//        while (elapsedTime < (recoilAmount / recoverySpeed))
//        {
//            recoilOffset = Mathf.Lerp(targetOffset, 0, elapsedTime / (recoilAmount / recoverySpeed));
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }
//        recoilOffset = 0;

//        isRecoiling = false;
//    }
//}

public class PlayerCamera : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private const float _threshold = 0.01f;
    public bool LockCameraPosition = false;
    public float sensitivity = 1f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // Recoil variables
    private float recoilOffset = 0f;
    [Header("Recoil Settings")]
    public float recoilAmount = 10f;     // How strong each shot's recoil is
    public float recoilRecovery = 5f;    // How quickly the camera recovers

    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degrees to override the camera. Useful for fine-tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    private PlayerInput _playerInput;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }
    }

    void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _input = GetComponent<StarterAssetsInputs>();
        _playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        CameraRotation();
        RecoverRecoil();  // Continually reduce recoilOffset over time
    }

    private void CameraRotation()
    {
        // Get player look input
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * sensitivity;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * sensitivity;
        }

        // Clamp the player's input-based pitch
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Final pitch = player input pitch + recoil offset
        float finalPitch = _cinemachineTargetPitch + recoilOffset;
        finalPitch = ClampAngle(finalPitch, BottomClamp, TopClamp);

        // Apply rotation
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(finalPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }

    public void AddRecoil()
    {
        // Immediately add recoil upwards (negative offset makes camera pitch go up)
        recoilOffset -= recoilAmount;
    }

    private void RecoverRecoil()
    {
        // Smoothly move recoilOffset toward 0 every frame
        recoilOffset = Mathf.Lerp(recoilOffset, 0f, Time.deltaTime * recoilRecovery);
    }
}