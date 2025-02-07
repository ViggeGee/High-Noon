using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class TrainSmoker : MonoBehaviour
{
    [SerializeField] float smokeDuration;
    [SerializeField] float minTimeBetweenSmoke;
    [SerializeField] float maxTimeBetweenSmoke;
    [SerializeField] AudioSource steamWhistle;
    [SerializeField] ParticleSystem steam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        StartCoroutine(TimeBetweenSmoke());
    }

    public IEnumerator TimeBetweenSmoke()
    {
        yield return new WaitForSeconds(Random.Range(minTimeBetweenSmoke, maxTimeBetweenSmoke));
        ActivateSmoke();
    }
    // Update is called once per frame
    public void ActivateSmoke()
    {
        steam.startColor = new Color(255, 255, 255, 255);
        steamWhistle.Play();
        StartCoroutine(DeactivateSmoke());
    }
    public IEnumerator DeactivateSmoke()
    {
        yield return new WaitForSeconds(smokeDuration);
        steam.startColor = new Color(255, 255, 255, 0);
        StartCoroutine(TimeBetweenSmoke());
    }
}
