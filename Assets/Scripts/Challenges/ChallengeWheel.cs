using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class ChallengeWheel : NetworkBehaviour
{
    [SerializeField] private RectTransform challengeWheelRect;
    [SerializeField] private Image pickChallengeArrow;
    [SerializeField] private Image[] availableChallenges;
    [SerializeField] private TextMeshProUGUI challengeText;

    private float rotatePower;
    private float stopPower;
    private float rotationSpeed;

    private bool isRotating = false;

    private float maxScale = 10f;
    private float scalingSpeed = 1f;

    public event Action<GameObject> OnChallengeSelected;

    private Canvas parentCanvas;

    private NetworkVariable<int> selectedChallengeIndex = new NetworkVariable<int>(-1); // Store challenge index
    private NetworkVariable<float> currentRotation = new NetworkVariable<float>(0);  // Sync rotation angle across the network

    private void Start()
    {
        challengeText.gameObject.SetActive(false);
        parentCanvas = transform.parent.gameObject.GetComponent<Canvas>();

        // Listen for challenge selection changes
        selectedChallengeIndex.OnValueChanged += (oldValue, newValue) =>
        {
            if (newValue != -1)
            {
                DisplayChallengeClientRpc(newValue);
            }
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsHost)
        {
            RotateServerRpc();
        }

        if (IsServer && isRotating)
        {
            // Rotate the wheel locally for the server
            rotationSpeed -= stopPower * Time.deltaTime;
            challengeWheelRect.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            currentRotation.Value = challengeWheelRect.eulerAngles.z;  // Sync the current rotation

            if (rotationSpeed <= 0)
            {
                rotationSpeed = 0;
                isRotating = false;
                DetermineChallengeServerRpc();
            }
        }
        else if (IsClient)
        {
            // Update the rotation on the client with the synced value
            challengeWheelRect.rotation = Quaternion.Euler(0, 0, currentRotation.Value);
        }
    }

    [ServerRpc]
    public void RotateServerRpc()
    {
        if (!isRotating)
        {
            stopPower = UnityEngine.Random.Range(300, 500);
            rotatePower = UnityEngine.Random.Range(1300, 1500);
            rotationSpeed = rotatePower;
            isRotating = true;
        }
    }

    [ServerRpc]
    private void DetermineChallengeServerRpc()
    {
        float rot = transform.eulerAngles.z;
        int challengeIndex = Mathf.FloorToInt(rot / 60f); // Divide wheel into 6 equal parts

        if (challengeIndex >= 0 && challengeIndex < availableChallenges.Length)
        {
            selectedChallengeIndex.Value = challengeIndex; // Sync challenge selection to clients
            SetCurrentChallengeServerRpc(challengeIndex);
        }
    }

    [ServerRpc]
    public void SetCurrentChallengeServerRpc(int challengeIndex)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentChallengeType.Value = availableChallenges[challengeIndex].GetComponent<Challenge>().challengeType;
        }
        else
        {
            Debug.LogError("GameManager is null");
        }
    }

    [ClientRpc]
    private void DisplayChallengeClientRpc(int challengeIndex)
    {
        if (challengeIndex >= 0 && challengeIndex < availableChallenges.Length)
        {
            challengeText.gameObject.SetActive(true);
            challengeText.text = availableChallenges[challengeIndex].GetComponent<Challenge>().challengeType.ToString();
            StartCoroutine(ScaleChallengeText());
        }
    }
    [ClientRpc]
    private void HideChallengeWheelClientRpc()
    {
        parentCanvas.gameObject.SetActive(false);  // Deactivate canvas on client side
    }


    private IEnumerator ScaleChallengeText()
    {
        Vector3 initialScale = Vector3.one;
        Vector3 targetScale = new Vector3(maxScale, maxScale, 1);

        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            challengeText.transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed);
            timeElapsed += Time.deltaTime * scalingSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(3);
        challengeText.transform.localScale = Vector3.one;
        challengeText.gameObject.SetActive(false);

        OnChallengeSelected?.Invoke(gameObject);

        HideChallengeWheelClientRpc();


    }
}



