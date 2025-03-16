using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ShootingGalleryManager : MonoBehaviour
{
    public Transform parentButtonList;
    public Transform parentSpawnPointList;
    public TextMeshProUGUI scoreText;

    private List<Button> targetButtonList;
    private List<Transform> spawnPointList;

    private float minDelay = 2.5f;
    private float maxDelay = 10f;
    private float activeTimeNormal = 0.8f;
    private float activeTimeSuper = 0.6f;

    private Dictionary<Button, bool> clickedFlags = new Dictionary<Button, bool>();
    [SerializeField] private Transform canvas;

    private int score = 0;
    private int normalAddScore = 1;
    private int superAddScore = 3;

    [HideInInspector] public bool gamemodeFinished = false;// HÄR ARN!
    private bool[] spawnOccupied;

    private void Start()
    {


        CreateLists();

        spawnOccupied = new bool[spawnPointList.Count];

        foreach (Button btn in targetButtonList)
        {
            btn.gameObject.SetActive(false);
            btn.onClick.AddListener(() => OnButtonClicked(btn));
        }

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
        Cursor.lockState = CursorLockMode.None;

        if (!GameManager.Instance.hasGameStarted.Value || !GameManager.Instance.isPlayer1Ready.Value || !GameManager.Instance.isPlayer2Ready.Value || GameManager.Instance.playerDied.Value) return;

        if (Input.GetMouseButtonDown(0) && !gamemodeFinished) 
        {
            ShootingGallerySFX.Instance.PlayLeftClick();
        }

        if (score >= 30f)
        {
            Cursor.lockState = CursorLockMode.Locked;
            gamemodeFinished = true;
            canvas.gameObject.SetActive(false);
            GameManager.Instance.readyToShoot = true;
            CinematicManager.Instance.StopCinematic();
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

    //RECEPTION FÖR LEDIGA PLATSER FÖR KNAPPAR ATT SPAWNA PÅ
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
            return -1; 

        int randomChoice = Random.Range(0, freeIndices.Count);
        return freeIndices[randomChoice];
    }

    //-----------AKTIVERINGSFUNKTION FÖR EN KNAPP
    //OBS!!!---BRICKAR NÅGON DETTA FÅR NI LÖSA DET SJÄLVA!-----------
    private IEnumerator ButtonRoutine(Button btn)
    {
        while (true)
        {
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            int spawnIndex = GetRandomFreeSpawnPointIndex();
            if (spawnIndex == -1)
            {
                // INGA LEDIGA PLATSER
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            spawnOccupied[spawnIndex] = true;

            Transform spawn = spawnPointList[spawnIndex];
            btn.transform.position = spawn.position;


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

            // TIDEN DÄR DEN SPAWNAR OCH DESPAWNAR AV SIG SJÄLV
            while (elapsed < chosenActiveTime && btn.gameObject.activeSelf && !clickedFlags[btn])
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // ROTATION PÅ TRÄFF
            if (clickedFlags[btn])
            {
                yield return StartCoroutine(RotateButton(btn, 1f, 720f));
            }

            btn.gameObject.SetActive(false);
            spawnOccupied[spawnIndex] = false;
        }
    }

    private void OnButtonClicked(Button clickedButton)
    {
        if (!GameManager.Instance.hasGameStarted.Value || !GameManager.Instance.isPlayer1Ready.Value || !GameManager.Instance.isPlayer2Ready.Value || GameManager.Instance.playerDied.Value) return;

        if (clickedButton.gameObject.activeSelf && !clickedFlags[clickedButton])
        {
            clickedFlags[clickedButton] = true; 
            ShootingGallerySFX.Instance.PlayHitTarget();
            if (clickedButton.CompareTag("GoodButton"))
            {
                score += normalAddScore;
            }
            else if (clickedButton.CompareTag("BadButton"))
            {
                score += superAddScore;
                ShootingGallerySFX.Instance.PlayRandomScream();
            }
            UpdateScoreText();

        }
    }

    private IEnumerator RotateButton(Button btn, float duration, float fullRotationAngle)
    {
        float elapsed = 0f;
        Quaternion originalRotation = btn.transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Lerp(0, fullRotationAngle, elapsed / duration);
            btn.transform.rotation = originalRotation * Quaternion.Euler(0, angle, 0);
            yield return null;
        }

        btn.transform.rotation = originalRotation;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
}
