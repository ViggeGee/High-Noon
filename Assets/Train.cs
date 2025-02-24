using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool spawnedTrain;
    [SerializeField] GameObject trainPrefab;
    [SerializeField] float xValueAtNewSpawn;
    [SerializeField] Vector3 spawnPosition;
    Quaternion spawnRotation;

    private void Start()
    {
        spawnRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
        SpawnNewTrain();
    }

    void SpawnNewTrain()
    {
        if (xValueAtNewSpawn > 0)
        {
            if (transform.position.x > xValueAtNewSpawn && !spawnedTrain)
            {
                GameObject newTrain = Instantiate(trainPrefab, spawnPosition, spawnRotation);
                spawnedTrain = true;

            }
        }
        else if (xValueAtNewSpawn < 0)
        {
            if (transform.position.x < xValueAtNewSpawn && !spawnedTrain)
            {
                GameObject newTrain = Instantiate(trainPrefab, spawnPosition, spawnRotation);
                spawnedTrain = true;

            }
        }

    }
}
