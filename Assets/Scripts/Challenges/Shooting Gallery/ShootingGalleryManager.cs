using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ShootingGalleryManager : MonoBehaviour
{
    public Transform parentButtonList;
    public Transform parentSpawnPointList;
    private List<Button> targetButtonList; // The buttons that will appear/disappear
    private List<Transform> spawnPointList; // 16 spawn points in the Inspector
    private float minDelay = 5f;
    private float maxDelay = 10f;
    private float activeTimeNormal = 0.8f;
    private float activeTimeSuper = 0.5f;
    private Dictionary<Button, bool> clickedFlags = new Dictionary<Button, bool>();

    public TextMeshProUGUI scoreText;
    private int score = 0;
    private int normalAddScore = 1;
    private int superAddScore = 3;

    // Array to track if each spawn point is occupied.
    private bool[] spawnOccupied;

    private void Start()
    {


        CreateLists();

        // Initialize our occupancy array with the same count as spawn points.
        spawnOccupied = new bool[spawnPointList.Count];

        // Deactivate all buttons and attach click listeners
        foreach (Button btn in targetButtonList)
        {
            btn.gameObject.SetActive(false);
            btn.onClick.AddListener(() => OnButtonClicked(btn));
        }

        // Start a coroutine for each button
        foreach (Button btn in targetButtonList)
        {
            StartCoroutine(ButtonRoutine(btn));
        }

        foreach (Button btn in targetButtonList)
        {
            clickedFlags[btn] = false;
        }
        UpdateScoreText();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            ShootingGallerySFX.Instance.PlayLeftClick();
        }
    }

    private void CreateLists()
    {
        targetButtonList = new List<Button>();
        spawnPointList = new List<Transform>();

        foreach (Transform child in parentButtonList)
        {
            Button btn = child.GetComponent<Button>();
            if (btn != null)
            {
                targetButtonList.Add(btn);
            }
        }
        foreach (Transform point in parentSpawnPointList)
        {
            spawnPointList.Add(point);
        }
    }

    // Helper function that returns a random free spawn point index, or -1 if none are free.
    private int GetRandomFreeSpawnPointIndex()
    {
        List<int> freeIndices = new List<int>();
        for (int i = 0; i < spawnOccupied.Length; i++)
        {
            if (!spawnOccupied[i])
            {
                freeIndices.Add(i);
            }
        }
        if (freeIndices.Count == 0)
            return -1; // No free spawn points available.

        int randomChoice = Random.Range(0, freeIndices.Count);
        return freeIndices[randomChoice];
    }

    private IEnumerator ButtonRoutine(Button btn)
    {
        while (true)
        {
            // Wait a random time before spawning.
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            int spawnIndex = GetRandomFreeSpawnPointIndex();
            if (spawnIndex == -1)
            {
                // No free spawn point; wait a short time and try again.
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            spawnOccupied[spawnIndex] = true;

            Transform spawn = spawnPointList[spawnIndex];
            btn.transform.position = spawn.position;

            // Reset the clicked flag and rotation for reuse.
            clickedFlags[btn] = false;
            btn.transform.rotation = Quaternion.identity;

            btn.gameObject.SetActive(true);

            float elapsed = 0f;
            float chosenActiveTime = 1f;
            
            if (btn.CompareTag("GoodButton"))
            {
                chosenActiveTime = activeTimeNormal;
            }
            else if (btn.CompareTag("BadButton"))
            {
                chosenActiveTime = activeTimeSuper;
            }

            // Wait until activeTime passes or the button gets clicked.
            while (elapsed < chosenActiveTime && btn.gameObject.activeSelf && !clickedFlags[btn])
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // If the button was clicked, run the rotation animation.
            if (clickedFlags[btn])
            {
                // Run rotation over 1 second.
                yield return StartCoroutine(RotateButton(btn, 1f, 720f));
            }

            btn.gameObject.SetActive(false);
            spawnOccupied[spawnIndex] = false;
        }
    }

    private void OnButtonClicked(Button clickedButton)
    {
        // Only award points if active and not already processed.
        if (clickedButton.gameObject.activeSelf && !clickedFlags[clickedButton])
        {
            clickedFlags[clickedButton] = true;  // Mark that this button was hit.

            if (clickedButton.CompareTag("GoodButton"))
            {
                score += normalAddScore;
                ShootingGallerySFX.Instance.PlayHitTarget();
            }
            else if (clickedButton.CompareTag("BadButton"))
            {
                score += superAddScore;
                ShootingGallerySFX.Instance.PlayRandomScream();
            }
            UpdateScoreText();

            // No longer deactivating or freeing the spawn point here!
        }
    }

    private IEnumerator RotateButton(Button btn, float duration, float fullRotationAngle)
    {
        float elapsed = 0f;
        Quaternion originalRotation = btn.transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Lerp the angle from 0 to the desired full rotation (e.g., 360 or 720 degrees).
            float angle = Mathf.Lerp(0, fullRotationAngle, elapsed / duration);
            // Apply the incremental rotation relative to the original.
            btn.transform.rotation = originalRotation * Quaternion.Euler(0, angle, 0);
            yield return null;
        }

        // Ensure the final rotation is exactly the original (or adjust if you want it to keep the rotation)
        btn.transform.rotation = originalRotation;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
