using Unity.Netcode;
using UnityEngine;


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
        }
    }
    private void Update()
    {
        if (!IsOwner) return;

        if(!playerAnimator.enabled)
        {
            shooterController.enabled = false;
            isPlayerDead = true;
        }
    }

}


