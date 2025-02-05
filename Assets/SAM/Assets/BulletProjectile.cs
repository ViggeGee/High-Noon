using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class BulletProjectile : NetworkBehaviour {

    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    private Animator animator;

    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner) // Only apply velocity on the owning client
        {
            float speed = 60f;
            bulletRigidbody.linearVelocity = transform.forward * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Ensure only the server handles hit detection

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player hit!");

            // Sync hit effect and disable player's animator
            HitPlayerClientRpc(other.GetComponent<NetworkObject>().NetworkObjectId);
        }
        else
        {
            // Sync hit effect
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
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerId];
        if (playerObject != null)
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