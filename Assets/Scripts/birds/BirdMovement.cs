using UnityEngine;
using Unity.Netcode;

public class BirdMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float stopThreshold = 0.01f;
    public float turnSmoothFactor = 2f;
    public float flyAwayDistance = 10f;
    public Vector3 areaSize = new Vector3(4, 3, 6);
    public float forwardOffset = 4f;
    public float  upOffset = 2f;

    private Transform target;
    private Animator animator;
    private Vector3 currentDestination;

    void Start()
    {
        target = GetNearestPlayer();
        animator = GetComponent<Animator>();
        currentDestination = GenerateRandomDestination();
    }

    Transform GetNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) return null;
        Transform nearest = players[0].transform;
        float minDist = Vector3.Distance(transform.position, nearest.position);
        for (int i = 1; i < players.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, players[i].transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = players[i].transform;
            }
        }
        return nearest;
    }

    Vector3 GenerateRandomDestination()
    {
        float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float randomY = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        float randomZ = Random.Range(-areaSize.z / 2f, areaSize.z / 2f);
        return new Vector3(randomX, randomY, randomZ);
    }

    void Update()
    {
        if (!IsServer) return;
        if (target != null)
        {
            TriggerBird trigger = target.GetComponent<TriggerBird>();
            Vector3 destination;
            if (trigger != null && trigger.triggerBirds)
            {
                Vector3 awayDir = (transform.position - target.position).normalized;
                destination = transform.position + awayDir * flyAwayDistance;
            }
            else
            {
                destination = target.position + target.forward * forwardOffset + target.up * upOffset  + target.TransformDirection(currentDestination);
            }
            Vector3 moveDir = destination - transform.position;
            if (moveDir.magnitude > stopThreshold)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                if (moveDir != Vector3.zero)
                {
                    Quaternion desiredRotation = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSmoothFactor * Time.deltaTime);
                }
                if (animator != null)
                    animator.SetBool("flying", true);
            }
            else
            {
                transform.position = destination;
                if (animator != null)
                    animator.SetBool("flying", false);
                if (trigger == null || !trigger.triggerBirds)
                    currentDestination = GenerateRandomDestination();
            }
        }
    }
}
