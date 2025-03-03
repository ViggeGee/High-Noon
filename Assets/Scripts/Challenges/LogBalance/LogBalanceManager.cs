using System.Collections;
using UnityEngine;

public class LogBalanceManager : MonoBehaviour
{

    [SerializeField] private Transform log;
    [SerializeField] private float challengeDuration;
    [SerializeField] private int minRotationSpeed;
    [SerializeField] private int maxRotationSpeed;
    private bool challengeOver = false;
    private bool challengeCompleted = false;


    public bool ChallengeOver { get { return challengeOver; } set { challengeOver = value; } }

    RotateLog logBehavior;
    void Start()
    {
        logBehavior = new RotateLog(log);
        StartCoroutine(RandomLogRotation());
    }

    // Update is called once per frame
    void Update()
    {
        if (challengeOver||challengeCompleted)//either if a player fell or if both players survived
        {
            foreach (var component in FindObjectsByType<PlayerLogSlip>(FindObjectsSortMode.None))
            {
                component.enabled = false; //Disables PlayerLogSlip component on both players
            }
            FindFirstObjectByType<Motion_Controller>().enabled = false;//Disables controller rotation
            StopAllCoroutines();//Stops log from rotation
            enabled = false;//Disables this component
        }
    }

    IEnumerator RandomLogRotation()//Coroutine that rotates the log a random amount of times for the duration of the challenge
    {
        float elapsedTime = 0f;
        while (elapsedTime < challengeDuration)
        {
            int rndRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
            int rndRotationDuration = Random.Range(1, (int)challengeDuration / 3); // Random duration of each individual rotation

            StartCoroutine(logBehavior.RotateLogMethod(rndRotationDuration, rndRotationSpeed));

            yield return new WaitForSeconds(rndRotationDuration);

            elapsedTime += rndRotationDuration;
        }
        //If both players survive the duration of the challenge the following code applies
        log.transform.rotation = Quaternion.Euler(0, 18.41f, 0);//Resets the log's rotation
        challengeCompleted = true;
        //Add code here for starting shooting
        if(GameManager.Instance != null)GameManager.Instance.readyToShoot = true;
    }

    //Check if a player hits the water
    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            
            challengeOver = true;
        }
    }
}
