using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Rewired;

public class BirdSpawner : NetworkBehaviour
{
    public GameObject birdPrefab;
    public int birdCount = 10;
    float spawnDelay = 10.0f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
       // yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count >= 2);
        yield return new WaitForSeconds(spawnDelay);
      
        for (int i = 0; i < birdCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 2f;
            GameObject bird = Instantiate(birdPrefab, spawnPosition, Quaternion.identity);
            bird.GetComponent<NetworkObject>().Spawn();
            bird.GetComponent<NetworkObject>().DestroyWithScene = true;
        }
    }
}
