using UnityEngine;
using UnityEngine.UIElements;

public class creditWalk : MonoBehaviour
{
    float moveSpeed = 0.05f;
    public GameObject gunshotBloodParticleEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position.z > -19)
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - moveSpeed);
        else
        {
            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                Animator animator = child.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("Idle", true);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);

                if (Physics.Raycast(ray, out hit)) // Check if we hit something
                {
                    if (hit.collider.CompareTag("NPC")) // Ensure it hits a plank
                    {
                        Instantiate(gunshotBloodParticleEffect, hit.point, Quaternion.identity);
                        hit.collider.gameObject.GetComponent<Animator>().enabled = false;
                    }
                }
            }
        }
    }
}
