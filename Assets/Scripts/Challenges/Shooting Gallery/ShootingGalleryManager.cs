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
    private float activeTime = 1f;

    public TextMeshProUGUI scoreText;
    private int score = 0;

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

            // Get a free spawn point index.
            int spawnIndex = GetRandomFreeSpawnPointIndex();
            if (spawnIndex == -1)
            {
                // No free spawn point; wait a short time and try again.
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            // Mark the spawn point as occupied.
            spawnOccupied[spawnIndex] = true;

            // Get the spawn point transform.
            Transform spawn = spawnPointList[spawnIndex];

            // Move the button to that spawn point.
            btn.transform.position = spawn.position;

            // Activate the button.
            btn.gameObject.SetActive(true);

            // Instead of waiting a fixed activeTime, check every frame.
            float elapsed = 0f;
            while (elapsed < activeTime && btn.gameObject.activeSelf)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Deactivate the button.
            btn.gameObject.SetActive(false);

            // Free up the spawn point.
            spawnOccupied[spawnIndex] = false;
        }
    }

    private void OnButtonClicked(Button clickedButton)
    {
        // Only award points if active.
        if (clickedButton.gameObject.activeSelf)
        {

            if (clickedButton.CompareTag("GoodButton"))
            {
                score++;
                ShootingGallerySFX.Instance.PlayHitTarget();
            }
            else if (clickedButton.CompareTag("BadButton"))
            {
                score--;
                ShootingGallerySFX.Instance.PlayRandomScream();
            }

            //StartCoroutine(RotateButton(clickedButton));
            UpdateScoreText();

            // Deactivate the button immediately after clicking.
            clickedButton.gameObject.SetActive(false);

            // Find which spawn point the button was occupying and free it.
            // This requires comparing positions (you could also store the index in a component for efficiency).
            for (int i = 0; i < spawnPointList.Count; i++)
            {
                if (Vector3.Distance(clickedButton.transform.position, spawnPointList[i].position) < 0.01f)
                {
                    spawnOccupied[i] = false;
                    break;
                }
            }
        }
    }

    private IEnumerator RotateButton(Button btn)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Quaternion startRotation = btn.transform.rotation;
        // Define the target rotation (rotate 180 degrees on Y axis, for example).
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 180, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            btn.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            
            yield return null;
        }
        // Ensure final rotation is set.
        btn.transform.rotation = targetRotation;
        //btn.gameObject.SetActive(false);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
