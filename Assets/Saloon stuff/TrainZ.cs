using UnityEngine;

public class TrainZ : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool spawnedTrain;
    [SerializeField] GameObject trainPrefab;
    [SerializeField] float zValueAtNewSpawn;
    [SerializeField] Vector3 spawnPosition;
    Quaternion spawnRotation;

    private void Start()
    {
        spawnRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x , transform.position.y, transform.position.z + speed);
        SpawnNewTrain();
    }

    void SpawnNewTrain()
    {
        if (zValueAtNewSpawn > 0)
        {
            if (transform.position.z > zValueAtNewSpawn && !spawnedTrain)
            {
                GameObject newTrain = Instantiate(trainPrefab, spawnPosition, spawnRotation);
                spawnedTrain = true;

            }
        }
        else if (zValueAtNewSpawn < 0)
        {
            if (transform.position.z < zValueAtNewSpawn && !spawnedTrain)
            {
                GameObject newTrain = Instantiate(trainPrefab, spawnPosition, spawnRotation);
                spawnedTrain = true;

            }
        }

    }
}
