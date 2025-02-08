using Unity.Netcode;
using UnityEngine;

public class NewGameManager : NetworkBehaviour
{
    public static NewGameManager Instance { get; private set; }
    public GameState currentGameState { get; private set; } = GameState.WaitingForPlayers;

    public NetworkVariable<bool> isPlayer1Ready = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isPlayer2Ready = new NetworkVariable<bool>(false);

    public NetworkVariable<bool> playerDied = new NetworkVariable<bool>(false);
    public NetworkVariable<NetworkObjectReference> playerThatDied = new NetworkVariable<NetworkObjectReference>();
    private bool displayGameOverCanvas = false;

    public NetworkVariable<bool> hasGameStarted = new NetworkVariable<bool>(false);

    public bool readyToShoot { get; set; } = false;

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
            if (IsServer)
            {
                playerDied.Value = false;
            }

            if (playerThatDied.Value.TryGet(out NetworkObject playerNetworkObject))
            {
                displayGameOverCanvas = true;
                UIManager.Instance.GetGameOverCanvas.gameObject.SetActive(true);
                // Access the GameObject from the NetworkObject
                GameObject playerGameObject = playerNetworkObject.gameObject;

                if (playerNetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    UIManager.Instance.GetLooseImage.gameObject.SetActive(true);
                }
                else
                {
                    UIManager.Instance.GetWinImage.gameObject.SetActive(true);
                }

                //if(IsHost)
                //{
                //    LoadNextLevelServerRpc();
                //}        
            }
            else
            {
                Debug.LogError("Failed to get the player network object.");
            }
        }
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
