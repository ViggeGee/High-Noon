using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class typeRacer : NetworkBehaviour
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
    //public bool readyToShoot = false;

    public AudioSource startGameSound;
    public AudioSource finnishRaceSound;

    [HideInInspector] public int nrFailLetters;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
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

        playerTyped = "";
    }


    public void PickRandomWord()
    {
        
        ClearWords();

        randomWord = wordsList[Random.Range(0, wordsList.Count)];

        float baseLetterSpacing = Screen.width * 0.03f; // Dynamic spacing based on screen width
        float spaceSpacing = baseLetterSpacing * 1.5f; // Extra spacing for spaces between words

        List<float> letterPositions = new List<float>();
        float totalWidth = 0f;

        // Calculate total width of the sentence
        for (int i = 0; i < randomWord.Length; i++)
        {
            if (randomWord[i] == ' ')
            {
                totalWidth += spaceSpacing;
            }
            else
            {
                totalWidth += baseLetterSpacing;
            }
            letterPositions.Add(totalWidth);
        }

        // Center the sentence on the X-axis
        float startX = spawnPoint.position.x - (totalWidth / 2f);

        for (int i = 0; i < randomWord.Length; i++)
        {
            if (randomWord[i] == ' ')
            {
                continue; // Skip rendering actual space characters
            }

            Vector3 letterPosition = new Vector3(startX + letterPositions[i], spawnPoint.position.y, spawnPoint.position.z);
            GameObject newLetter = Instantiate(letterPrefab, letterPosition, Quaternion.identity, canvas.transform);

            char letter = char.ToLower(randomWord[i]); // Ensure lowercase lookup
            if (letterDictionary.ContainsKey(letter))
            {
                newLetter.GetComponent<Image>().sprite = letterDictionary[letter];
            }

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
        if (playerTyped.Replace(" ", "") == randomWord.ToUpper().Replace(" ", ""))
        {
            Debug.Log("Correct! Word completed: " + randomWord);
            playerInput.text = ""; // Clear input for next word
            completedWords++;
           
            if(completedWords >= 1)
            {
                finnishRaceSound.Play();
                GameManager.Instance.readyToShoot = true;
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

