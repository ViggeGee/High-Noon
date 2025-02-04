using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using static Challenge;

public enum GameState
{
    MainMenu,
    WaitingForPlayers,
    Playing
}

/// <summary>
/// This script handles the game loop 
/// </summary>
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState currentGameState { get; private set; } = GameState.WaitingForPlayers;

    private const int MAX_NUMBER_OF_PLAYERS = 2;

    public static bool bHasGameStarted = false;
    private static int playersJoined = 0;

    private bool challengeSelected = false;

    #region UI variables

    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private Canvas challengeCanvas;

    public bool bIsPlayer1Ready { get; private set; } = false;
    public bool bIsPlayer2Ready { get; private set; } = false;

    #endregion

    #region Challenge

    [SerializeField] private GameObject[] availableChallenges;

    private ChallengeWheel challengeWheel;

    public NetworkVariable<ChallengeType> currentChallengeType = new NetworkVariable<ChallengeType>();

    private NetworkVariable<bool> countdownStarted = new NetworkVariable<bool>(false);

    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
                StartCoroutine(CountDown());
        countdownStarted.OnValueChanged += (oldValue, newValue) =>
        {
            if (newValue)
            {
                StartCoroutine(CountDown());

                // Once countdown starts, trigger the countdown function
            }
        };

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnect;
        }
    }
    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnect;

        challengeWheel.OnChallengeSelected -= ChallengeWheel_OnChallengeSelected;
    }

    private void Update()
    {
        switch(currentGameState)
        {
            case GameState.MainMenu:

                break;


            case GameState.WaitingForPlayers:

                if(playersJoined == 2)
                {
                    challengeWheel = FindFirstObjectByType<ChallengeWheel>();

                    if(challengeWheel != null)
                    {
                        challengeWheel.OnChallengeSelected += ChallengeWheel_OnChallengeSelected;
                        challengeWheel.RotateServerRpc();
                        
                        currentGameState = GameState.Playing;
                    }
                    else
                    {
                        Debug.LogError("ChallengeWheel is null");
                    }
                }

                break;

            case GameState.Playing:


                break;
        }
    }


    #region Countdown and challenge selection

    private void StartCountDown()
    {
        countdownStarted.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartCountDownServerRpc()
    {
        StartCountDown();  // Call the original function on the server
    }
    private IEnumerator CountDown()
    {
        
        yield return StartCoroutine(FadeText("3"));
        yield return StartCoroutine(FadeText("2"));
        yield return StartCoroutine(FadeText("1"));
        yield return StartCoroutine(FadeText("DRAW"));

        countDownText.text = "";
        //startGameSound.Play();
        bHasGameStarted = true;
        if(OwnerClientId == 0)
        {
            bIsPlayer1Ready = true;
        }
        else if(OwnerClientId == 1)
        {
            bIsPlayer2Ready = true;
        }
    }
    IEnumerator FadeText(string newText)
    {
        // Scale down
        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(1, 0, t / 0.2f);
            countDownText.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        countDownText.transform.localScale = Vector3.zero;

        // Change the text
        countDownText.text = newText;

        // Scale back up
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(0, 1, t / 0.3f);
            countDownText.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        countDownText.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(1.5f); // Stay visible before next fade
    }

    [ServerRpc]
    private void ToggleChallengeCanvasServerRpc()
    {
        ToggleChallengeCanvasClientRpc();
    }

    [ClientRpc]
    private void ToggleChallengeCanvasClientRpc()
    {
        challengeCanvas.gameObject.SetActive(true);

        foreach (GameObject challenge in availableChallenges)
        {
            if (currentChallengeType.Value == Challenge.ChallengeType.typeRacer)
            {

                if (challenge.CompareTag("TypeRacer"))
                {
                    challenge.SetActive(true);
                }
            }
        }

        StartCountDownServerRpc();
    }

    #endregion

    #region Events

    private void Singleton_OnClientConnect(ulong clientId)
    {
        if(playersJoined < MAX_NUMBER_OF_PLAYERS)
        {
            Debug.Log($"Player{clientId} joined");
            playersJoined++;
        }
    }
    private void Singleton_OnClientDisconnect(ulong clientId)
    {
        Debug.Log($"Player{clientId} disconnected");
        playersJoined--;
    }
    
    private void ChallengeWheel_OnChallengeSelected(GameObject @object)
    {
        if (!IsHost) return;

        ToggleChallengeCanvasServerRpc();
    }

    

    #endregion

}
