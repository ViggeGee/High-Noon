using UnityEngine;

public class RotateLog
{

    private Transform transform;
    private float rotationSpeed = 30;

    public RotateLog( Transform transform)
    {
        this.transform = transform;
    }

    // Update is called once per frame
    public void RotateLogMethod()
    {
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}
