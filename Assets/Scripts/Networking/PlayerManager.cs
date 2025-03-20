using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public const int MAX_NUMBER_OF_PLAYERS = 2;

    public event Action<int> OnPlayersJoined;
    public int PlayersJoined => playersJoined.Value;
    private NetworkVariable<int> playersJoined = new NetworkVariable<int>(0);

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject player1SpawnPoint, player2SpawnPoint;

    private bool hasDeactivatedSpawnPoints = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if(!hasDeactivatedSpawnPoints)
        {
            hasDeactivatedSpawnPoints = true;

            player1SpawnPoint.SetActive(false);
            player2SpawnPoint.SetActive(false);
        }
    }

    public void HandlePlayerJoin(ulong clientId)
    {
        if (IsServer)
        {
            if (playersJoined.Value >= 2) return;

            Transform spawnPoint = (playersJoined.Value == 0) ? player1SpawnPoint.transform : player2SpawnPoint.transform;
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            player.GetComponent<NetworkObject>().DestroyWithScene = true;

            Player playerScript = player.GetComponent<Player>();
            playerScript.scoreData.scorePlayer1 = 0;
            playerScript.scoreData.scorePlayer2 = 0;
            playersJoined.Value++;

            OnPlayersJoined?.Invoke(playersJoined.Value);
        }

        
    }

    public void HandlePlayerSpawnOnSceneChange()
    {
       
        if(IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                Motion_Controller motionController = FindAnyObjectByType<Motion_Controller>();
                
                ulong clientId = client.Key; // ClientId is the dictionary key

                if (playersJoined.Value >= 2) return;

                Transform spawnPoint = (playersJoined.Value == 0) ? player1SpawnPoint.transform : player2SpawnPoint.transform;
                GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
                player.GetComponent<NetworkObject>().DestroyWithScene = true;

                if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LogBalanceScene"))
                {
                    if (motionController != null)
                    {
                        Debug.Log($"[HandlePlayerSpawn] Assigning Player {(int)clientId + 1} to Motion_Controller");
                        motionController.AssignPlayer(player, (int)clientId + 1);
                    }
                    else
                    {
                        Debug.LogError("[HandlePlayerSpawn] Motion_Controller not found. Player assignment skipped.");
                    }
                }
                


                playersJoined.Value++;

                OnPlayersJoined?.Invoke(playersJoined.Value);
            }
        }

       
    }

    //public void HandlePlayerSpawnOnSceneChange()
    //{
    //    if (!IsServer) return;

    //    //Motion_Controller motionController = FindAnyObjectByType<Motion_Controller>();

    //    int currentPlayerIndex = playersJoined.Value + 1;

    //    Transform spawnPoint = (currentPlayerIndex == 1) ? player1SpawnPoint.transform : player2SpawnPoint.transform;

    //    GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    //    player.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
    //    player.GetComponent<NetworkObject>().DestroyWithScene = true;

    //    Debug.Log($"[HandlePlayerSpawn] Spawned Player {currentPlayerIndex} at {spawnPoint.position}");

    //    //// Assign the player to Motion_Controller
    //    //if (motionController != null)
    //    //{
    //    //    Debug.Log($"[HandlePlayerSpawn] Assigning Player {currentPlayerIndex} to Motion_Controller");
    //    //    motionController.AssignPlayer(player, currentPlayerIndex);
    //    //}
    //    //else
    //    //{
    //    //    Debug.LogError("[HandlePlayerSpawn] Motion_Controller not found. Player assignment skipped.");
    //    //}

    //    playersJoined.Value++;
    //    OnPlayersJoined?.Invoke(playersJoined.Value);
    //}

    public void HandlePlayerDisconnect(ulong clientId)
    {
        if(!IsServer) return;   

        playersJoined.Value--;
    }

    
}

