using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public AudioSource gunShotAudio;
    public AudioSource explosionAudio;
    public GameObject[] planks;
    public GameObject[] barrels;
    int index = 0;
    public GameObject gunshotParticleEffect; // Assign your particle prefab in the inspector
    public GameObject explosionParticleEffect; // Assign your particle prefab in the inspector


    public void BreakPlank(int i)
    {
        gunShotAudio.Play();
        StartCoroutine(Plankfalling(i));
        if (i == 0)
            StartCoroutine(StartGame());
    }

    public void BreakBarrel(int i)
    {
        gunShotAudio.Play();
        StartCoroutine(ExplodeBarrel(i));
    }

    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2f);
        Cursor.visible = true;
        SceneLoader.Instance.LoadScene(Scenes.WesternTown);
        //SceneManager.LoadScene("Generic standoff level_MULTIPLAYER");
    }

    public IEnumerator Plankfalling(int i)
    {
        yield return new WaitForSeconds(0.6f);
        SpawnParticleAtCursor();
        if (planks[i].GetComponent<SpringJoint>() != null)
        planks[i].GetComponent<SpringJoint>().connectedBody = null;

        planks[i + 1].GetComponent<Rigidbody>().freezeRotation = false;
        planks[i + 1].GetComponent<Rigidbody>().isKinematic = false;
        planks[i+1].GetComponent<Rigidbody>().AddForce(new Vector3(0,5,5),ForceMode.Impulse);
        planks[i+1].GetComponent<Rigidbody>().AddTorque(new Vector3(0,1,0),ForceMode.Impulse);
    }

    public IEnumerator ExplodeBarrel(int i)
    {
        yield return new WaitForSeconds(0.6f);

        explosionAudio.Play();
        Instantiate(explosionParticleEffect, barrels[i].transform.position, Quaternion.identity);
        barrels[i].gameObject.SetActive(false);
    }

    void SpawnParticleAtCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Check if we hit something
        {
            if (hit.collider.CompareTag("Plank")) // Ensure it hits a plank
            {
                Instantiate(gunshotParticleEffect, hit.point, Quaternion.identity);
            }
        }
    }
}
