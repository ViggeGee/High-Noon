using System.Collections;
using UnityEngine;

public class GunshotManager : MonoBehaviour
{
    public GameObject[] gunshots;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        StartCoroutine(playEffect(minTime, maxTime));
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator playEffect(float minTime, float maxTime)
    {
        float timeBetween = Random.Range(minTime, maxTime);
        yield return new WaitForSeconds(timeBetween);
        int i = Random.Range(0, gunshots.Length);
        gunshots[i].GetComponent<ParticleSystem>().Play();
        StartCoroutine(playEffect(minTime, maxTime));
    }
}
