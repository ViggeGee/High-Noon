using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkEventManager : NetworkBehaviour
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

        SceneLoader.Instance.LoadScene(Scenes.MainMenu);
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

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Scenes.MainMenu.ToString())) return;

        gameManagerInstance = Instantiate(gameManagerPrefab);
        NetworkObject networkObject = gameManagerInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();
        networkObject.DestroyWithScene = true;
    }

    private void HandleSceneEvent(SceneEvent sceneEvent)
    {

        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.Load:

                break;

            case SceneEventType.LoadComplete:
                // Scene fully loaded
                SpawnGameManager();
                PlayerManager.Instance.HandlePlayerSpawnOnSceneChange();
                GameManager.Instance.UpdateCurrentGameStateServerRpc(GameState.ChoosingChallenge);
               
                break;

            case SceneEventType.Unload:
            
                break;

            case SceneEventType.UnloadComplete:
                
                break;
        }
    }

    private void OnClientConnected(ulong clientId) => PlayerManager.Instance.HandlePlayerJoin(clientId);
    private void OnClientDisconnected(ulong clientId) => PlayerManager.Instance.HandlePlayerDisconnect(clientId);
}
