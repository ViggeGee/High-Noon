using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
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

    #region Challenges

    private typeRacer typeRacer;

    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //challengeWheel = FindFirstObjectByType<ChallengeWheel>();
        if (challengeWheel != null) challengeWheel.OnChallengeSelected += ChallengeWheel_OnChallengeSelected;

        PlayerManager.Instance.OnPlayersJoined += PlayerManager_OnPlayersJoined;
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

        //// Add other mistake references for other challenges here
    }

    private void PlayerManager_OnPlayersJoined(int numberOfPlayers)
    {
        if (numberOfPlayers == PlayerManager.MAX_NUMBER_OF_PLAYERS)
        {
            if(IsServer)
            {
                challengeWheel.RotateServerRpc();
                NewGameManager.Instance.UpdateCurrentGameStateServerRpc(GameState.ChoosingChallenge);
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
        }

        UIManager.Instance.StartCountDownServerRpc();
    }
}

