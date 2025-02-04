using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class typeRacer : MonoBehaviour
{
    public Sprite[] letterextures;  // Assign in Inspector (A-Z)
    public TextAsset textAsset;

    public Canvas canvas;

    public GameObject letterPrefab;
    private List<GameObject> PrefabLettersInWord = new List<GameObject>();
    private List<char> charLettersInWord = new List<char>();
    public InputField playerInput;

    public string randomWord = "Not Working";
    private Dictionary<char, Sprite> letterDictionary = new Dictionary<char, Sprite>();
    private List<string> wordsList = new List<string>();

    public Transform spawnPoint; // Set a spawn point in the scene

    //public TextMeshProUGUI canvasText;

    int completedWords = 0;
    string playerTyped;
    //bool gameStarted = false;
    public bool readyToShoot = false;

    public AudioSource startGameSound;
    public AudioSource finnishRaceSound;

    [HideInInspector] public int nrFailLetters;

    void Start()
    {
        //StartCoroutine(CountDown());
        LoadWordsFromFile();
        // Populate dictionary (Assumes prefab names are "A", "B", "C", etc.)
        foreach (Sprite texture in letterextures)
        {
            char letter = texture.name[0]; // Get first character from prefab name
            letterDictionary[letter] = texture;
        }
        PickRandomWord();

    }

    

    //IEnumerator FadeText(string newText)
    //{
    //    // Scale down
    //    for (float t = 0; t < 0.2f; t += Time.deltaTime)
    //    {
    //        float scale = Mathf.Lerp(1, 0, t / 0.2f);
    //        canvasText.transform.localScale = new Vector3(scale, scale, scale);
    //        yield return null;
    //    }
    //    canvasText.transform.localScale = Vector3.zero;

    //    // Change the text
    //    canvasText.text = newText;

    //    // Scale back up
    //    for (float t = 0; t < 0.3f; t += Time.deltaTime)
    //    {
    //        float scale = Mathf.Lerp(0, 1, t / 0.3f);
    //        canvasText.transform.localScale = new Vector3(scale, scale, scale);
    //        yield return null;
    //    }
    //    canvasText.transform.localScale = Vector3.one;

    //    yield return new WaitForSeconds(1.5f); // Stay visible before next fade
    //}

    public void PickRandomWord()
    {
        ClearWords();

        randomWord = wordsList[Random.Range(0, wordsList.Count)];

        for (int i = 0; i < randomWord.Length; i++)
        {
            GameObject newLetter = Instantiate(letterPrefab, new Vector3(spawnPoint.position.x + (i * 80), spawnPoint.position.y, spawnPoint.position.z), Quaternion.identity, canvas.transform);

            char letter = randomWord[i];

            newLetter.GetComponent<Image>().sprite = letterDictionary[letter];

            PrefabLettersInWord.Add(newLetter);
            charLettersInWord.Add(letter);

        }
        playerInput.onValueChanged.AddListener(delegate { CheckInput(); });
    }


    void LoadWordsFromFile()
    {

        if (textAsset != null)
        {
            string[] words = textAsset.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            wordsList.AddRange(words);
            Debug.Log("Loaded " + wordsList.Count + " words!");
        }
        else
        {
            Debug.LogError("Words file not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.bHasGameStarted || !GameManager.Instance.bIsPlayer1Ready || GameManager.Instance.bIsPlayer2Ready) return;

        playerInput.Select();

        // Check if player finished the word
        if (playerTyped == randomWord.ToUpper())
        {
            Debug.Log("Correct! Word completed: " + randomWord);
            playerInput.text = ""; // Clear input for next word
            completedWords++;
            if (completedWords <= 2)
            {
                PickRandomWord();
            }
            if(completedWords >= 3)
            {
                finnishRaceSound.Play();
                readyToShoot = true;
                ClearWords();
            }
        }

    }

    public void ClearWords()
    {
        if (charLettersInWord.Count > 0)
        {
            charLettersInWord.Clear();
        }
        if (PrefabLettersInWord.Count > 0)
        {
            for (int i = 0; i < PrefabLettersInWord.Count; i++)
            {
                PrefabLettersInWord[i].gameObject.SetActive(false);
            }
            PrefabLettersInWord.Clear();
        }
    }

    void CheckInput()
    {
       playerTyped = playerInput.text.ToUpper();

        for (int i = 0; i < PrefabLettersInWord.Count; i++)
        {
            Image letterImage = PrefabLettersInWord[i].GetComponent<Image>();

            if (i < playerTyped.Length)
            {
                if (char.ToLower(playerTyped[i]) == charLettersInWord[i])
                {
                    letterImage.color = Color.green; // Correct letter
                }
                else if (char.ToLower(playerTyped[i]) != charLettersInWord[i])
                {
                    letterImage.color = Color.red; // Incorrect letter
                    nrFailLetters++;
                    
                }
            }
            else
            {
                letterImage.color = Color.white; // Reset color for remaining letters
            }
        }

        
    
    }

 

}
