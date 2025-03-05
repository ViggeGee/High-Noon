using UnityEngine;
using UnityEngine.AI;

public class RandomWalk : MonoBehaviour
{
    [SerializeField] private Transform[] walkPoints; // Array of designated walk points
    [SerializeField] private float wanderInterval = 2f; // How often to pick a new point

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderInterval;
        setNewDestination();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Check if the agent is close to its destination or if it's time to pick a new one
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            setNewDestination();
            timer = 0;
        }

        // Optional: Force a new destination after a certain interval
        if (timer >= wanderInterval)
        {
            setNewDestination();
            timer = 0;
        }
    }

    private void setNewDestination()
    {
        if (walkPoints.Length == 0) return;

        // Pick a random point from the array of walkPoints
        int randomIndex = Random.Range(0, walkPoints.Length);
        Vector3 targetPosition = walkPoints[randomIndex].position;

        agent.SetDestination(targetPosition);
    }
}
