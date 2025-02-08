using System.Collections;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    public static CinematicManager Instance { get; private set; }

    public GameObject[] cinematicCameras;
    public GameObject cinematicCanvas;

    private bool hasDeactivatedCinematic = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator PlayCinematic()
    {
        if (cinematicCameras.Length == 0) yield return null;

        while (!NewGameManager.Instance.readyToShoot) 
        {
            for (int i = 0; i < cinematicCameras.Length; i++)
            {
                if (NewGameManager.Instance.readyToShoot) break;

                // Activate the current camera and deactivate all others
                for (int j = 0; j < cinematicCameras.Length; j++)
                {
                    if (cinematicCameras[j] != null)
                        cinematicCameras[j].SetActive(j == i);
                }

                yield return new WaitForSeconds(4f); // Wait 1 second before switching
            }
        }

        // Deactivate all cameras once readyToShoot is true

        yield return null;

    }

    public void StopCinematic()
    {
        if (cinematicCameras.Length == 0) return;

        if (NewGameManager.Instance.readyToShoot && !hasDeactivatedCinematic)
        {
            StopCoroutine(PlayCinematic());
            Cursor.lockState = CursorLockMode.Locked;
            foreach (GameObject cam in cinematicCameras)
            {
                cam.SetActive(false);
            }
            cinematicCanvas.SetActive(false);
            hasDeactivatedCinematic = true;
        }
    }

    public IEnumerator WaitForChallengeInitialization(GameObject challenge, Challenge.ChallengeType currentChallenge)
    {
        if (ChallengeManager.Instance.currentChallengeType.Value == Challenge.ChallengeType.typeRacer)
        {
            yield return new WaitUntil(() => challenge.GetComponent<typeRacer>() != null);
            typeRacer typeRacer = challenge.GetComponent<typeRacer>();
            typeRacer.SetNetworkSentenceServerRpc();
            NewGameManager.Instance.UpdateCurrentGameStateServerRpc(GameState.Playing);
        }
    }
}

