using Newtonsoft.Json.Bson;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeWheel : MonoBehaviour
{
    [SerializeField] private RectTransform challengeWheelRect;
    [SerializeField] private Image pickChallengeArrow;
    [SerializeField] private Image[] availableChallenges;
    [SerializeField] private TextMeshProUGUI challengeText;

    private float rotatePower;
    private float stopPower; 
    private float rotationSpeed;

    private bool isRotating = false;
    private bool challengeSet = false;

    private void Start()
    {
        challengeText.gameObject.SetActive(false);
        Rotate();
    }

    private float maxScale = 10f;  // Set the maximum scale factor
    private float scalingSpeed = 1f; // Speed at which the object scales up
    private Vector3 targetScale = Vector3.zero; // To store the target scale during scaling

    void Update()
    {
        if (isRotating)
        {
            challengeWheelRect.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            rotationSpeed -= stopPower * Time.deltaTime;  // Slow down rotation

            if (rotationSpeed <= 0)
            {
                rotationSpeed = 0;
                isRotating = false;
                SetCurrentChallenge();
            }
        }

        if(challengeSet)
        {
            targetScale = Vector3.Lerp(challengeText.transform.localScale, new Vector3(maxScale, maxScale, 1), Time.deltaTime * scalingSpeed);
            challengeText.transform.localScale = Vector3.ClampMagnitude(targetScale, maxScale);

            if(challengeText.transform.localScale == targetScale)
            {
                StartCoroutine(DisableChallengeWheel());
            }
        }
    }

    public void Rotate()
    {
        if (!isRotating)
        {
            stopPower = Random.Range(300, 500);
            rotatePower = Random.Range(1300, 1500);
            rotationSpeed = rotatePower;
            isRotating = true;
        }
    }

    private Challenge.ChallengeType ReturnPickedChallenge()
    {
        float rot = transform.eulerAngles.z;

        if (rot > 0 && rot <= 60)
        {
            challengeText.gameObject.SetActive(true);
            challengeSet = true;
            challengeText.text = availableChallenges[0].gameObject.GetComponent<Challenge>().challengeType.ToString();
            return availableChallenges[0].gameObject.GetComponent<Challenge>().challengeType;
        }
        else if (rot > 60 && rot <= 120)
        {
            challengeText.gameObject.SetActive(true);
            challengeSet = true;
            challengeText.text = availableChallenges[1].gameObject.GetComponent<Challenge>().challengeType.ToString();
            return availableChallenges[1].gameObject.GetComponent<Challenge>().challengeType;
        }
        else if (rot > 120 && rot <= 180)
        {
            challengeText.gameObject.SetActive(true);
            challengeSet = true;
            challengeText.text = availableChallenges[2].gameObject.GetComponent<Challenge>().challengeType.ToString();
            return availableChallenges[2].gameObject.GetComponent<Challenge>().challengeType;
        }
        else if (rot > 180 && rot <= 240)
        {
            challengeText.gameObject.SetActive(true);
            challengeSet = true;
            challengeText.text = availableChallenges[3].gameObject.GetComponent<Challenge>().challengeType.ToString();
            return availableChallenges[3].gameObject.GetComponent<Challenge>().challengeType;
        }
        else if (rot > 240 && rot <= 300)
        {
            challengeText.gameObject.SetActive(true);
            challengeSet = true;
            challengeText.text = availableChallenges[4].gameObject.GetComponent<Challenge>().challengeType.ToString();
            return availableChallenges[4].gameObject.GetComponent<Challenge>().challengeType;
        }
        else if (rot > 300 && rot <= 360)
        {
            challengeText.gameObject.SetActive(true);
            challengeSet = true;
            challengeText.text = availableChallenges[5].gameObject.GetComponent<Challenge>().challengeType.ToString();
            return availableChallenges[5].gameObject.GetComponent<Challenge>().challengeType;
        }
        else
        {
            return Challenge.ChallengeType.ERROR;
        }
    }

    public void SetCurrentChallenge()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentChallengeType = ReturnPickedChallenge();
        }
        else
        {
            Debug.LogError("GameManager is null");
        }
    }
    
    private IEnumerator DisableChallengeWheel()
    {
        yield return new WaitForSeconds(3);
        transform.parent.gameObject.SetActive(false);
    }
}
