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
            playerAnimator = GetComponent<Animator>();
            shooterController = GetComponent<ShooterController>();
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
        GameManager.Instance.playerDied.Value = true;
        GameManager.Instance.playerThatDied.Value = gameObject.GetComponent<NetworkObject>();
    }

    
}


