using UnityEngine;

public class LogBalanceManager : MonoBehaviour
{

    [SerializeField] private Transform log;

    RotateLog logBehavior;
    void Start()
    {
        logBehavior = new RotateLog(log);
    }

    // Update is called once per frame
    void Update()
    {
        logBehavior.RotateLogMethod();
    }
}
