using UnityEngine;
using Unity.Netcode;

public class BirdSpawner : NetworkBehaviour
{
    public GameObject birdPrefab;
    public int birdCount = 10;
    private void Start()
    {
        
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        Debug.Log("BirdSpawner: Spawning birds...");

        for (int i = 0; i < birdCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 2f;
            GameObject bird = Instantiate(birdPrefab, spawnPosition, Quaternion.identity);
            bird.GetComponent<NetworkObject>().Spawn();
        }
    }
}
