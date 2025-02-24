using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Challenge;

public class ChallengeManager : NetworkBehaviour
{
    public static ChallengeManager Instance { get; private set; }

    [SerializeField] private GameObject[] availableChallenges;
    [SerializeField] private Canvas challengeWheelCanvas;
    [SerializeField] private ChallengeWheel challengeWheel;

    public NetworkVariable<ChallengeType> currentChallengeType = new NetworkVariable<ChallengeType>();
    private GameObject currentChallengeObject;

    public int mistakesDuringChallenge = 0;

    private bool hasStartedWheelRotation = false;

    #region Challenges

    private typeRacer typeRacer;
    private ButtonSmashManager buttonSmash;

    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Scenes.JoinGameScene.ToString()))
        {
            challengeWheelCanvas.gameObject.SetActive(false);
        }

        //challengeWheel = FindFirstObjectByType<ChallengeWheel>();
        if (challengeWheel != null) challengeWheel.OnChallengeSelected += ChallengeWheel_OnChallengeSelected;

        PlayerManager.Instance.OnPlayersJoined += PlayerManager_OnPlayersJoined;
    }

    private void Update()
    {
        StartWheelRotation();
    }

    public void SetMistakesDuringChallenge()
    {
        if (currentChallengeType.Value == Challenge.ChallengeType.typeRacer)
        {
            if (typeRacer != null)
            {
                mistakesDuringChallenge = typeRacer.nrFailLetters;
            }
        }
        else if (currentChallengeType.Value == Challenge.ChallengeType.ButtonSmash)
        {
            if (buttonSmash != null)
            {
                // NR OF MISTAKES FOR BUTTON SMASH
            }
        }

        //// Add other mistake references for other challenges here
    }

    private void StartWheelRotation()
    {
        if (!IsServer) return;

        if(PlayerManager.Instance.PlayersJoined == 2 && !hasStartedWheelRotation)
        {
            hasStartedWheelRotation = true;
            challengeWheel.RotateServerRpc();
            GameManager.Instance.UpdateCurrentGameStateServerRpc(GameState.ChoosingChallenge);
        }
    }
    private void PlayerManager_OnPlayersJoined(int numberOfPlayers) ///// PLACE THIS METHOD SOMEWHERE MORE APPROPRIATE IN THE FUTURE
    {
        if (numberOfPlayers == PlayerManager.MAX_NUMBER_OF_PLAYERS)
        {
            if(IsServer)
            {
                if (GameManager.Instance.currentGameState == GameState.StartGameScene)
                {
                    SceneLoader.Instance.LoadRandomSceneForAllPlayers();
                }
            }
            
            
        }
    }

    private void ChallengeWheel_OnChallengeSelected(GameObject challenge)
    {
        if (!IsServer) return;

        ToggleChallengeCanvasServerRpc();

    }


    [ServerRpc]
    private void ToggleChallengeCanvasServerRpc()
    {
        ToggleChallengeCanvasClientRpc();
    }

    [ClientRpc]
    private void ToggleChallengeCanvasClientRpc()
    {
        challengeWheel.gameObject.SetActive(true);

        foreach (GameObject challenge in availableChallenges)
        {
            if (currentChallengeType.Value == Challenge.ChallengeType.typeRacer)
            {

                if (challenge.CompareTag("TypeRacer"))
                {
                    challenge.SetActive(true);
                    typeRacer = challenge.GetComponent<typeRacer>();
                    StartCoroutine(CinematicManager.Instance.PlayCinematic());
                    StartCoroutine(CinematicManager.Instance.WaitForChallengeInitialization(challenge, currentChallengeType.Value));
                }
            }
            else if (currentChallengeType.Value == Challenge.ChallengeType.ButtonSmash)
            {

                if (challenge.CompareTag("ButtonSmash"))
                {
                    challenge.SetActive(true);
                    buttonSmash = challenge.GetComponent<ButtonSmashManager>();
                    StartCoroutine(CinematicManager.Instance.PlayCinematic());
                    StartCoroutine(CinematicManager.Instance.WaitForChallengeInitialization(challenge, currentChallengeType.Value));
                }
            }
        }

        UIManager.Instance.StartCountDownServerRpc();
    }
}

