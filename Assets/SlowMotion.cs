using System.Collections;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
  

    private void Start()
    {

    }
    public IEnumerator StartSlowMotion(float slowMotionTimeInSeconds, float timeScale)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSeconds(slowMotionTimeInSeconds);
        Time.timeScale = 1;
    }
}
