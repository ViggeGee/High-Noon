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

    public NetworkVariable<bool> isPlayer1Ready = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isPlayer2Ready = new NetworkVariable<bool>(false);

    public NetworkVariable<bool> playerDied = new NetworkVariable<bool>(false);
    public NetworkVariable<NetworkObjectReference> playerThatDied = new NetworkVariable<NetworkObjectReference>();
    private bool displayGameOverCanvas = false;

    public NetworkVariable<bool> hasGameStarted = new NetworkVariable<bool>(false);

    public bool readyToShoot { get; set; } = false;

    private NetworkVariable<bool> hasSomeoneWon = new NetworkVariable<bool>(false);
    
    private bool hasStoppedCinematic;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

                break;
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

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerScoreServerRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            Player player = networkClient.PlayerObject.GetComponent<Player>();
            if (player != null)
            {
                if(clientId == 1)
                {
                    player.scoreData.scorePlayer1 += 1;
                }
                else
                {
                    player.scoreData.scorePlayer2 += 1;
                }
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
