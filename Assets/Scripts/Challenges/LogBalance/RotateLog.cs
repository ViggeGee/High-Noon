using System.Collections;
using UnityEngine;

public class RotateLog
{

    private Transform transform;
    private int direction = 1;
    public RotateLog( Transform transform)
    {
        this.transform = transform;
    }

    // Update is called once per frame
    public IEnumerator RotateLogMethod(int rotationDuration , int rotationSpeed)
    {
        float elapsedTime = 0f;
        while(elapsedTime < rotationDuration)
        {
            transform.Rotate(Vector3.right *direction * rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        direction *= -1;
    }
}
