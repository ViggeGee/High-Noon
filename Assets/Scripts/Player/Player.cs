using System;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class Player : NetworkBehaviour
{
    public enum DamageType
    {
        None,

        Head,
        RightLeg,
        LeftLeg,
        RightArm,
        LeftArm,
        Stomach,
        Chest
    }

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ShooterController shooterController;
    private bool isPlayerDead = false;

    private float health = 100f;

  
    private void Update()
    {

        if(health <= 0)
        {
            SetDiedVariablesServerRpc();
        }
    }

    public void TakeDamage(DamageType damageType)
    {  
        switch(damageType)
        {
            case DamageType.Head:

                health = 0;
                Debug.Log("HEAD");
                break;

            case DamageType.RightLeg:

                health -= 30f;
                Debug.Log("RIGHT LEG");
                break;

            case DamageType.LeftLeg:
                health -= 30f;
                Debug.Log("LEFT LEG");
                break;

            case DamageType.RightArm:

                health -= 20f;
                Debug.Log("RIGHT ARM");
                break;

            case DamageType.LeftArm:
                health -= 20f;
                Debug.Log("LEFT ARM");
                break;

            case DamageType.Stomach:

                health -= 50;
                Debug.Log("STOMACH");
                break;

            case DamageType.Chest:

                health -= 70;
                Debug.Log("CHEST");
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDiedVariablesServerRpc()
    {
        DieClientRPC();
        GameManager.Instance.playerDied.Value = true;
        GameManager.Instance.playerThatDied.Value = gameObject.GetComponent<NetworkObject>();
        
    }

    [ClientRpc]
    private void DieClientRPC()
    {
        playerAnimator.enabled = false;
        shooterController.enabled = false;
        isPlayerDead = true;
    }
}


