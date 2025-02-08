using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public const int MAX_NUMBER_OF_PLAYERS = 2;

    public event Action<int> OnPlayersJoined;
    public int PlayersJoined => playersJoined.Value;
    private NetworkVariable<int> playersJoined = new NetworkVariable<int>(0);

    [SerializeField] private GameObject playerPrefab;
    private GameObject player1SpawnPoint, player2SpawnPoint;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        player1SpawnPoint = GameObject.FindGameObjectWithTag("Player1Spawn");
        player2SpawnPoint = GameObject.FindGameObjectWithTag("Player2Spawn");
    }

    public void HandlePlayerJoin(ulong clientId)
    {
        if (IsServer)
        {
            if (playersJoined.Value >= 2) return;

            Transform spawnPoint = (playersJoined.Value == 0) ? player1SpawnPoint.transform : player2SpawnPoint.transform;
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            playersJoined.Value++;

            OnPlayersJoined?.Invoke(playersJoined.Value);
        }

        player1SpawnPoint.SetActive(false);
        player2SpawnPoint.SetActive(false);
    }

    public void HandlePlayerDisconnect(ulong clientId)
    {
        if(!IsServer) return;   

        playersJoined.Value--;
    }

    
}

