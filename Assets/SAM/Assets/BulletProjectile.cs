using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {

    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    private Animator animator;

    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 60f;
        bulletRigidbody.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.GetComponent<BulletTarget>() != null) {
        //    // Hit target
        //    Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
        //}
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player entered the trigger!");
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
            animator = other.GetComponent<Animator>();
            animator.enabled = false;
        }
        else
        
        {
            // Hit something else
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

}