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
        if (challengeOver)
        {
            StopAllCoroutines();
        }
    }

    IEnumerator RandomLogRotation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < challengeDuration)
        {
            int rndRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
            int rndRotationDuration = Random.Range(1, (int)challengeDuration / 2);

            StartCoroutine(logBehavior.RotateLogMethod(rndRotationDuration, rndRotationSpeed));

            yield return new WaitForSeconds(rndRotationDuration);

            elapsedTime += rndRotationDuration;
        }

        challengeCompleted = true;
        if(GameManager.Instance != null)GameManager.Instance.readyToShoot = true;
    }

    
}
