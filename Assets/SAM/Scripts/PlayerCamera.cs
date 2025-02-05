using StarterAssets;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;
using System.Collections;



using Unity.Netcode;

public class PlayerCamera : NetworkBehaviour // ✅ Make it networked
{
    private StarterAssetsInputs _input;
    private const float _threshold = 0.01f;
    public bool LockCameraPosition = false;
    public float sensitivity = 1f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private float recoilOffset = 0f; // New variable to track recoil separately

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

    private bool isRecoiling = false;
    private float recoilAmount = 10f;
    private float recoilSpeed = 100f;
    private float recoverySpeed = 50f;

    //void Start()
    //{
    //    // ✅ Only initialize camera for the owner (local player)
       

        
    //}
    public override void OnNetworkSpawn()
{
    if (IsOwner)
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        
        // 🛠 Assign inputs only for the local player (to avoid sharing input across players)
        _input = GetComponent<StarterAssetsInputs>();
            _input.enabled = true;
        if (_input == null)
        {
            Debug.LogError($"[PlayerCamera] OwnerClientId: {OwnerClientId} - StarterAssetsInputs NOT found!");
        }

        _playerInput = GetComponent<PlayerInput>();
            _playerInput.enabled = true;
        if (_playerInput == null)
        {
            Debug.LogError($"[PlayerCamera] OwnerClientId: {OwnerClientId} - PlayerInput NOT found!");
        }
    }
    else
    {
        // ❗ Disable input handling for non-owner players
        enabled = false;
    }
}


    void Update()
    {
        if (!IsOwner || !GameManager.Instance.readyToShoot) return; // ✅ Ensure only local player can rotate camera
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (_input == null)
        {
            Debug.LogError($"[PlayerCamera] OwnerClientId: {OwnerClientId} - _input is null!");
            return;
        }

        Debug.Log($"[PlayerCamera] OwnerClientId: {OwnerClientId} - Look Input: {_input.look}, Magnitude: {_input.look.sqrMagnitude}");

        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * sensitivity;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * sensitivity;

            Debug.Log($"[PlayerCamera] OwnerClientId: {OwnerClientId} - Updated Yaw: {_cinemachineTargetYaw}, Pitch: {_cinemachineTargetPitch}");
        }

        // Clamp the player's input-based pitch
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Apply recoil offset **without overriding player input**
        float finalPitch = _cinemachineTargetPitch + recoilOffset;

        // Ensure final pitch stays within bounds
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
        if (!isRecoiling)
        {
            StartCoroutine(ApplyRecoil());
        }
    }

    private IEnumerator ApplyRecoil()
    {
        isRecoiling = true;
        float targetOffset = -recoilAmount; // Negative makes the camera go UP
        float elapsedTime = 0f;

        // Move camera up with recoil effect
        while (elapsedTime < (recoilAmount / recoilSpeed))
        {
            recoilOffset = Mathf.Lerp(0, targetOffset, elapsedTime / (recoilAmount / recoilSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        recoilOffset = targetOffset;

        // Wait briefly at the top
        yield return new WaitForSeconds(0.1f);

        // Move camera back down (smooth recovery)
        elapsedTime = 0f;
        while (elapsedTime < (recoilAmount / recoverySpeed))
        {
            recoilOffset = Mathf.Lerp(targetOffset, 0, elapsedTime / (recoilAmount / recoverySpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        recoilOffset = 0;

        isRecoiling = false;
    }
}
