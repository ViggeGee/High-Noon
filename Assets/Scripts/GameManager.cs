using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum GameState
{
    MainMenu,
    StartGameScene,
    WaitingForPlayers,
    ChoosingChallenge,
    Playing
}

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState currentGameState { get; private set; } = GameState.StartGameScene;

    public NetworkVariable<bool> player1OutOfBullets = new NetworkVariable<bool>();
    public NetworkVariable<bool> player2OutOfBullets = new NetworkVariable<bool>();
    private bool isTie = false;

    public NetworkVariable<bool> isPlayer1Ready = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isPlayer2Ready = new NetworkVariable<bool>(false);

    public NetworkVariable<bool> playerDied = new NetworkVariable<bool>(false);
    public NetworkVariable<NetworkObjectReference> playerThatDied = new NetworkVariable<NetworkObjectReference>();
    private bool displayGameOverCanvas = false;

    public NetworkVariable<bool> hasGameStarted = new NetworkVariable<bool>(false);

    public bool readyToShoot { get; set; } = false;

    //public NetworkVariable<bool> readyToShoot = 
    //    new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<bool> hasSomeoneWon = new NetworkVariable<bool>(false);
    
    private bool hasStoppedCinematic;

    private NetworkVariable<int> player1Score = new NetworkVariable<int>(0);
    private NetworkVariable<int> player2Score = new NetworkVariable<int>(0);
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Listen for changes in score and update UI automatically
        player1Score.OnValueChanged += UpdateScoreUI;
        player2Score.OnValueChanged += UpdateScoreUI;
    }

    private void Update()
    {
        switch (currentGameState)
        {
            case GameState.StartGameScene:


                break;

            case GameState.WaitingForPlayers:
     
                    
                break;

            case GameState.ChoosingChallenge:


                break;

            case GameState.Playing:

                ChallengeManager.Instance.SetMistakesDuringChallenge();
                HandlePlayerDeath();
                HandlePlayerTie();
                break;
        }
    }

    private void HandlePlayerTie()
    {
        if(player1OutOfBullets.Value && player2OutOfBullets.Value && !displayGameOverCanvas)
        {
            isTie = true;
            Time.timeScale = 0.2f;

            displayGameOverCanvas = true;

            UIManager.Instance.GetGameOverCanvas.gameObject.SetActive(true);

            UIManager.Instance.GetTieImage.gameObject.SetActive(true);

            if (IsHost)
            {
                StartCoroutine(DelayedCheckWin());
            }
        }
    }

    private void HandlePlayerDeath()
    {
        if (playerDied.Value == true && !displayGameOverCanvas)
        {
            Time.timeScale = 0.2f;

            if (playerThatDied.Value.TryGet(out NetworkObject playerNetworkObject))
            {
                displayGameOverCanvas = true;
                UIManager.Instance.GetGameOverCanvas.gameObject.SetActive(true);

                Player player = playerNetworkObject.gameObject.GetComponent<Player>();

                if (playerNetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    UIManager.Instance.GetLooseImage.gameObject.SetActive(true);                                           
                }
                else
                {
                    UpdatePlayerScoreServerRpc(NetworkManager.Singleton.LocalClientId);
                    UIManager.Instance.GetWinImage.gameObject.SetActive(true);      
                }

                if (IsHost)
                {
                    StartCoroutine(DelayedCheckWin());
                }

                
            }
            else
            {
                Debug.LogError("Failed to get the player network object.");
            }
        }
    }

    private void UpdateScoreUI(int oldValue, int newValue)
    {
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            UIManager.Instance.scoreWinScreen.text = player1Score.Value.ToString() + " of 3";
            UIManager.Instance.scoreLooseScreen.text = player1Score.Value.ToString() + " of 3";
        }
        else
        {
            UIManager.Instance.scoreWinScreen.text = player2Score.Value.ToString() + " of 3";
            UIManager.Instance.scoreLooseScreen.text = player2Score.Value.ToString() + " of 3";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerScoreServerRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            Player player = networkClient.PlayerObject.GetComponent<Player>();
            if (player != null)
            {
                if(!isTie)
                {
                    if (clientId == 0)
                    {
                        player.scoreData.scorePlayer1 += 1;

                    }
                    else
                    {
                        player.scoreData.scorePlayer2 += 1;

                    }
                }
                
                player1Score.Value = player.scoreData.scorePlayer1;
                player2Score.Value = player.scoreData.scorePlayer2;
            }
        }
    }

    private IEnumerator DelayedCheckWin()
    {
        yield return new WaitForSeconds(0.5f);
        hasSomeoneWon.Value = CheckIfSomeoneWon();

        LoadNextLevelServerRpc();
        playerDied.Value = false; // Moved here
    }

    private bool CheckIfSomeoneWon()
    {
        List<ulong> clientIds = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);

        foreach (var clientId in clientIds)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
            {
                GameObject playerObject = networkClient.PlayerObject.gameObject;
                Player player = playerObject.GetComponent<Player>();

                if(player.scoreData.scorePlayer1 >= 3 || player.scoreData.scorePlayer2 >= 3)
                {
                    return true;
                }
            }
            
        }
        return false;
    }

    [ServerRpc]
    private void LoadNextLevelServerRpc()
    {
        if (!hasSomeoneWon.Value)
        {
            StartCoroutine(LoadNextSceneTimer());
        }
        else
        {
            StartCoroutine(LoadMainMenuTimer());
        }

    }

    private IEnumerator LoadNextSceneTimer()
    {
        yield return new WaitForSecondsRealtime(6);
        ResetTimeScaleClientRpc();
        SceneLoader.Instance.LoadRandomSceneForAllPlayers();
    }

    public IEnumerator LoadMainMenuTimer()
    {
        yield return new WaitForSecondsRealtime(6);
        ResetTimeScaleClientRpc();
       
        SceneLoader.Instance.LoadMainMenuAfterGameIsOver();
    }


    [ClientRpc]
    private void ResetTimeScaleClientRpc()
    {
        Time.timeScale = 1f;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateCurrentGameStateServerRpc(GameState newGameState)
    {
        UpdateGameStateClientRpc(newGameState);
    }

    [ClientRpc]
    private void UpdateGameStateClientRpc(GameState newGameState)
    {
        currentGameState = newGameState;
    }
   
}
