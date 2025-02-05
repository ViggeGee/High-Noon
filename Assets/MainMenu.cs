using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class MainMenu : MonoBehaviour
{
    public AudioSource gunShotAudio;
    public GameObject[] planks;
    int index = 0;
    
    public void BreakPlank(int i)
    {
        gunShotAudio.Play();
        StartCoroutine(Plankfalling(i));
    }

    public IEnumerator Plankfalling(int i)
    {
        yield return new WaitForSeconds(1.6f);
        planks[i].GetComponent<SpringJoint>().connectedBody = null;
        planks[i + 1].GetComponent<Rigidbody>().freezeRotation = false;
        planks[i+1].GetComponent<Rigidbody>().AddForce(new Vector3(0,5,5),ForceMode.Impulse);

    }

}
