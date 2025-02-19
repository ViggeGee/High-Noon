using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class BulletProjectile : NetworkBehaviour
{

    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    private Rigidbody bulletRigidbody;
    private float speed = 40f;

    //public static event Action<GameObject>
    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Only the server controls movement
        {
            bulletRigidbody.linearVelocity = transform.forward * speed;
        }
    }

    private void FixedUpdate()
    {
        if (IsServer) // Only the server updates movement
        {
            bulletRigidbody.linearVelocity = transform.forward * speed;
        }
    }

    Player.DamageType damageType;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Ensure only the server handles collisions

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Transform rootParent = GetRootParent(other.transform); // Get the highest parent
            NetworkObject networkObject = rootParent.GetComponent<NetworkObject>();

            damageType = HitPoint(other.tag);

            if (networkObject != null)  // Ensure networkObject is valid
            {
                     
                //other.enabled = false;
                ulong playerId = networkObject.NetworkObjectId;

                HitPlayerClientRpc(playerId);               
            }
            else
            {
                Debug.LogWarning("Player hit, but NetworkObject is missing. Maybe it was disabled or destroyed?");
            }
        }
        else
        {
            // Sync hit effect if not a player
            ShowHitEffectClientRpc(false, transform.position);
        }
        DestroyBulletServerRpc();
    }
    private Transform GetRootParent(Transform child)
    {
        while (child.parent != null)
        {
            child = child.parent;
        }
        return child; // Returns the top-most parent
    }
    private Player.DamageType HitPoint(string tag)
    {
        if(tag == "Head")
        {
            return Player.DamageType.Head;
        }
        else if(tag == "RightArm")
        {
            return Player.DamageType.RightArm;
        }
        else if(tag == "LeftArm")
        {
            return Player.DamageType.LeftArm;
        }
        else if(tag == "RightLeg")
        {
            return Player.DamageType.RightLeg;
        }
        else if(tag == "LeftLeg")
        {
            return Player.DamageType.LeftLeg;
        }
        else if(tag == "Stomach")
        {
            return Player.DamageType.Stomach;
        }
        else if(tag == "Chest")
        {
            return Player.DamageType.Chest;
        }

        return Player.DamageType.None;
    }

    [ClientRpc]
    private void ShowHitEffectClientRpc(bool hitPlayer, Vector3 position)
    {
        Instantiate(hitPlayer ? vfxHitGreen : vfxHitRed, position, Quaternion.identity);
    }

    [ClientRpc]
    private void HitPlayerClientRpc(ulong playerId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
        {
        
            Player player = playerObject.GetComponent<Player>();
            
            if(player != null)
            {
                player.TakeDamage(damageType);
            }

           
            foreach (Rigidbody rb in playerObject.GetComponentsInChildren<Rigidbody>())
            {
                if (rb.gameObject.name == "Head")
                {                 
                    rb.AddForce(bulletRigidbody.angularVelocity * 50);
                    break;
                }
            }
        }

        ShowHitEffectClientRpc(true, transform.position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyBulletServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
