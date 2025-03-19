using System.Collections;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class LogBalanceManager : NetworkBehaviour
{

    [SerializeField] private Transform log;
    [SerializeField] private float challengeDuration;
    [SerializeField] private int minRotationSpeed;
    [SerializeField] private int maxRotationSpeed;

    [SerializeField] private TextMeshProUGUI tmp_instructions;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI challengeDurationText;
    [SerializeField] private Canvas UICanvas;

    private bool challengeOver = false;
    private bool challengeActive = false;
    private bool challengeCompleted = false;

    private NetworkVariable<float> countdown = new NetworkVariable<float>(5); // Store challenge index
    private NetworkVariable<float> challengeDurationTimer = new NetworkVariable<float>(0); // Store challenge index
    private bool ChallengeOver { get { return challengeOver; } set { challengeOver = value; } }

    RotateLog logBehavior;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        logBehavior = new RotateLog(log);

        // Subscribe to countdown changes for all clients
        countdown.OnValueChanged += OnCountdownChanged;
        challengeDurationTimer.OnValueChanged += OnChallengeDurationChanged;

        if (IsServer)
        {
            GameManager.Instance.UpdateCurrentGameStateServerRpc(GameState.Playing);
            StartCoroutine(RandomLogRotation());
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            countdown.Value -= Time.deltaTime;
            countdownText.text = Mathf.CeilToInt(countdown.Value).ToString(); // Rounds up to whole number
        }



        if (countdown.Value >= 0) return;
        countdownText.gameObject.SetActive(false);
        if(challengeActive)
        {
            if (IsServer)
            {
                challengeDurationTimer.Value += Time.deltaTime;
            }
        }
       
        

        if (challengeOver||challengeCompleted)//either if a player fell or if both players survived
        {
            foreach (var component in FindObjectsByType<PlayerLogSlip>(FindObjectsSortMode.None))
            {
                component.enabled = false; //Disables PlayerLogSlip component on both players
            }
            FindFirstObjectByType<Motion_Controller>().enabled = false;//Disables controller rotation
            StopAllCoroutines();//Stops log from rotation
            enabled = false;//Disables this component
        }
    }

    private void OnCountdownChanged(float oldValue, float newValue)
    {
        countdownText.text = Mathf.CeilToInt(newValue).ToString(); // Update UI on all clients
    }
    private void OnChallengeDurationChanged(float oldValue, float newValue)
    {
        challengeDurationText.text = Mathf.CeilToInt(newValue).ToString(); // Update UI on all clients
    }

    IEnumerator RandomLogRotation()//Coroutine that rotates the log a random amount of times for the duration of the challenge
    {
        while (countdown.Value > 0)
        {
            yield return null;
        }
        challengeDurationText.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < challengeDuration)
        {
            challengeActive = true;
            int rndRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
            int rndRotationDuration = Random.Range(1, (int)challengeDuration / 3); // Random duration of each individual rotation

            StartCoroutine(logBehavior.RotateLogMethod(rndRotationDuration, rndRotationSpeed));

            yield return new WaitForSeconds(rndRotationDuration);

            elapsedTime += rndRotationDuration;
            challengeDurationText.text = Mathf.CeilToInt(elapsedTime).ToString();
        }
        //If both players survive the duration of the challenge the following code applies
        //log.transform.rotation = Quaternion.Euler(0, 18.41f, 0);//Resets the log's rotation
        
        //Add code here for starting shooting
        CountdownFinishedServerRpc();
       
    }

    [ServerRpc]
    private void CountdownFinishedServerRpc()
    {
        GameManager.Instance.isPlayer1Ready.Value = true;
        GameManager.Instance.isPlayer2Ready.Value = true;
        GameManager.Instance.hasGameStarted.Value = true;

        CountdownFinishedClientRpc(); 
    }
    [ClientRpc]
    private void CountdownFinishedClientRpc()
    {
        challengeCompleted = true;

        if (GameManager.Instance != null)
        {
            UICanvas.gameObject.SetActive(false);
            GameManager.Instance.readyToShoot = true;
        }

    }

    //Check if a player hits the water
    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        { 
            challengeOver = true;
        }
    }
}
