using System;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class Player : NetworkBehaviour
{
    private Animator playerAnimator;
    private ShooterController shooterController;
    private bool isPlayerDead = false;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            transform.rotation = Quaternion.identity;
            playerAnimator = GetComponent<Animator>();
            shooterController = GetComponent<ShooterController>();

            NetworkObject networkObject = GetComponent<NetworkObject>();
            networkObject.DestroyWithScene = true;
        }
    }
    private void Update()
    {
        if (!IsOwner) return;

        if(!playerAnimator.enabled)
        {
            shooterController.enabled = false;
            isPlayerDead = true;

            SetDiedVariablesServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDiedVariablesServerRpc()
    {
        NewGameManager.Instance.playerDied.Value = true;
        NewGameManager.Instance.playerThatDied.Value = gameObject.GetComponent<NetworkObject>();
    }

    
}


