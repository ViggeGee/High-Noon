using System;
using Unity.Netcode;
using UnityEngine;

public class ExtendedNetworkManager : NetworkBehaviour
{
    public GameObject gameManagerPrefab;
    private GameObject gameManagerInstance;
   
    private void Start()
    {
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
            gameManagerInstance = Instantiate(gameManagerPrefab);
            NetworkObject networkObject = gameManagerInstance.GetComponent<NetworkObject>();
            networkObject.Spawn();
        } 
    }

    private void OnClientConnected(ulong clientId) => PlayerManager.Instance.HandlePlayerJoin(clientId);
    private void OnClientDisconnected(ulong clientId) => PlayerManager.Instance.HandlePlayerDisconnect(clientId);
}
