using UnityEngine;

public class ActivateParticles : MonoBehaviour
{
    public GameObject[] particles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Activate()
    {
        for (int i = 0; i < particles.Length; i++)
        {

            particles[i].SetActive(true);
            particles[i].GetComponent<ParticleSystem>().Play();
        }
    }
}
