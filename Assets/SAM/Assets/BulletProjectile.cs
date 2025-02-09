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

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Ensure only the server handles collisions

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            NetworkObject networkObject = other.GetComponent<NetworkObject>();

            if (networkObject != null)  // Ensure networkObject is valid
            {
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
