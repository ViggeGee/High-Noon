using UnityEngine;

public class rotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public bool rotateX;
   public bool rotateY;
   public bool rotateZ;

    [SerializeField] int speed; 

    // Update is called once per frame
    void FixedUpdate()
    {
        if(rotateX)
        transform.Rotate(0.01f * speed, 0, 0);
        if(rotateY)
        transform.Rotate(0, 0.01f * speed, 0);
        if(rotateZ)
        transform.Rotate(0, 0, 0.5f * speed);
    }
}
