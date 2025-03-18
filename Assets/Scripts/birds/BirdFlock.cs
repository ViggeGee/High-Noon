using Unity.Netcode;
using UnityEngine;

public class BirdFlock : NetworkBehaviour
{

    public Animator birdAnimator;


 
  

    public void StartFlyingInFrontOfPlayer(Transform playerTransform)
    {
    
        Vector3 newPosition = playerTransform.position + playerTransform.forward * 5f;
        transform.position = newPosition;
       
    }
}
