using System;
using Unity.Netcode;
using UnityEngine;

public class ExtendedNetworkManager : NetworkBehaviour
{
    public GameObject gameManagerPrefab;
    private GameObject gameManagerInstance;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnServerStopped += HandleServerStopped;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    

    public override void OnDestroy()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
            NetworkManager.Singleton.OnServerStopped -= HandleServerStopped;

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

            NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;
        }
    }

    private void HandleServerStopped(bool obj)
    {
        if (!IsHost) return;

        if (gameManagerInstance != null)
        {
            Destroy(gameManagerInstance);
        }
    }

    private void HandleServerStarted()
    {
        if (!IsHost) return;

        if(gameManagerInstance == null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;

            SpawnGameManager();
        } 
    }

    private void SpawnGameManager()
    {
        if (!IsHost) return; 

        gameManagerInstance = Instantiate(gameManagerPrefab);
        NetworkObject networkObject = gameManagerInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();
        networkObject.DestroyWithScene = true;
    }

    private void HandleSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
        {
            SpawnGameManager();
            PlayerManager.Instance.HandlePlayerSpawnOnSceneChange();
            NewGameManager.Instance.UpdateCurrentGameStateServerRpc(GameState.ChoosingChallenge);
        }
        else if (sceneEvent.SceneEventType == SceneEventType.UnloadComplete)
        {
            
        }
        else if (sceneEvent.SceneEventType == SceneEventType.Load)
        {
            
        }
    }

    private void OnClientConnected(ulong clientId) => PlayerManager.Instance.HandlePlayerJoin(clientId);
    private void OnClientDisconnected(ulong clientId) => PlayerManager.Instance.HandlePlayerDisconnect(clientId);
}
