using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    
    public AudioSource gunShotAudio;
    public AudioSource explosionAudio;
    public AudioSource deathAudio;
    public GameObject[] planks;
    public GameObject[] barrels;
    public GameObject cowboy;
    int index = 0;
    public GameObject gunshotParticleEffect; // Assign your particle prefab in the inspector
    public GameObject gunshotBloodParticleEffect; // Assign your particle prefab in the inspector
    public GameObject explosionParticleEffect; // Assign your particle prefab in the inspector
    public GameObject waterParticleEffect; // Assign your particle prefab in the inspector

    public GameObject titleButton;
    public GameObject waterTowerButton;

    public GameObject creditsGO;

    private PlayerJoined playerJoined;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None; 

        playerJoined = FindAnyObjectByType<PlayerJoined>();
    }

    public void BreakPlank(int i)
    {
        gunShotAudio.Play();
        StartCoroutine(Plankfalling(i));
        if (i == 0)
        {
            StartCoroutine(StartGame());
        }
        else if(i == 1)
        {
            StartCoroutine(JoinGame());
        }
        else if(i == 2)
        {
            StartCoroutine(SpawnCredits());
        }
        else if (i == 3)
        {
            StartCoroutine(QuitGame());

        }
        else if (i == 4)
        {
            titleButton.SetActive(false);
            waterTowerButton.SetActive(true);
        }
    }

    public void BreakBarrel(int i)
    {
       
        gunShotAudio.Play();
        StartCoroutine(ExplodeBarrel(i));
    }
    public void ShootWaterTower()
    {
        gunShotAudio.Play();
        StartCoroutine(SpawnWaterParticleAtCursor());
    }
    public void KillCowboy()
    {
        gunShotAudio.Play();
        StartCoroutine(CowboyDeath());
    }

    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2f);
        Cursor.visible = true;
        playerJoined.SetIsPlayerHost(true);
        SceneLoader.Instance.LoadScene(Scenes.JoinGameScene);
        //SceneManager.LoadScene("Generic standoff level_MULTIPLAYER");
    }
    
    public IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(2f);
        Application.Quit();
        //SceneManager.LoadScene("Generic standoff level_MULTIPLAYER");
    }
    

    public IEnumerator JoinGame()
    {
        yield return new WaitForSeconds(2f);
        Cursor.visible = true;
        playerJoined.SetIsPlayerHost(false);
        SceneLoader.Instance.LoadScene(Scenes.JoinGameScene);
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
    public IEnumerator CowboyDeath()
    {
        yield return new WaitForSeconds(0.6f);
        SpawnBloodParticleAtCursor();
        deathAudio.Play();
        cowboy.GetComponent<Animator>().enabled = false;

        cowboy.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0, 150, 150), ForceMode.Impulse);
        cowboy.GetComponentInChildren<Rigidbody>().AddTorque(new Vector3(0, 1, 0), ForceMode.Impulse);

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
    IEnumerator SpawnWaterParticleAtCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        yield return new WaitForSeconds(0.6f);
        if (Physics.Raycast(ray, out hit)) // Check if we hit something
        {
            if (hit.collider.CompareTag("WaterTower")) // Ensure it hits a plank
            {
                Quaternion lookRotation = Quaternion.LookRotation(Camera.main.transform.position - hit.point, Vector3.up);
                Instantiate(waterParticleEffect, hit.point, Quaternion.Euler(90, lookRotation.eulerAngles.y, lookRotation.eulerAngles.z));
            }
        }
    }
    
    IEnumerator SpawnCredits()
    {
      
        yield return new WaitForSeconds(0.6f);
        creditsGO.SetActive(true);
    }
    void SpawnBloodParticleAtCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Check if we hit something
        {
            if (hit.collider.CompareTag("NPC")) // Ensure it hits a plank
            {
                Instantiate(gunshotBloodParticleEffect, hit.point, Quaternion.identity);
            }
        }
    }
}
