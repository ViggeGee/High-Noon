using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraTest : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private const float _threshold = 0.01f;
    public bool LockCameraPosition = false;
    public float sensitivity = 1f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private float recoilOffset = 0f; // New variable to track recoil separately

    [Header("Recoil settings")]
    [SerializeField] private float recoilRecovery = 5f;
    [SerializeField] private float recoilAmount = 10f;

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
    void Update()
    {
        //if (!IsOwner || !GameManager.Instance.readyToShoot) return; 
        
        CameraRotation();
        RecoverRecoil();
    }

    private void CameraRotation()
    {

        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * sensitivity;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * sensitivity;

        }

        // Clamp the player's input-based pitch
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Apply recoil offset **without overriding player input**
        float finalPitch = _cinemachineTargetPitch + recoilOffset;
        finalPitch = ClampAngle(finalPitch, BottomClamp, TopClamp);

        // Apply rotation to the camera
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
        recoilOffset -= recoilAmount;
    }

    private void RecoverRecoil()
    {
        recoilOffset = Mathf.Lerp(recoilOffset, 0f, Time.deltaTime * recoilRecovery);
    }
}
