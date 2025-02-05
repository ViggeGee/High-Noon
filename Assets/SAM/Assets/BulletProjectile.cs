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
    private float speed = 60f;

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

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Ensure only the server handles collisions

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player hit!");

            // Get the NetworkObject ID of the player that was hit
            ulong playerId = other.GetComponent<NetworkObject>().NetworkObjectId;  // Use OwnerClientId instead of NetworkObjectId

            // Sync hit effect and trigger the player hit event
            HitPlayerClientRpc(playerId);
        }
        else
        {
            // Sync hit effect if not a player
            ShowHitEffectClientRpc(false, transform.position);
        }

        // Destroy bullet across the network
        DestroyBulletServerRpc();
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
            Animator animator = playerObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
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
