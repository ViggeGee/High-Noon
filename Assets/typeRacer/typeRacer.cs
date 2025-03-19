using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class typeRacer : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> networkSentence = new NetworkVariable<FixedString64Bytes>(
    "",
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
);


    public Sprite[] letterextures;  // Assign in Inspector (A-Z)
    public TextAsset textAsset;

    public Canvas canvas;

    public GameObject letterPrefab;
    private List<GameObject> PrefabLettersInWord = new List<GameObject>();
    private List<char> charLettersInWord = new List<char>();

    private List<GameObject> PrefabLettersInWordOpponent = new List<GameObject>();
    private List<char> charLettersInWordOpponent = new List<char>();

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

        if (!IsHost)
        {
            networkSentence.OnValueChanged += OnNetworkSentenceChanged;
        }
        //StartCoroutine(CountDown());
        LoadWordsFromFile();
        // Populate dictionary (Assumes prefab names are "A", "B", "C", etc.)
        foreach (Sprite texture in letterextures)
        {
            char letter = texture.name[0]; // Get first character from prefab name
            letterDictionary[letter] = texture;
        }

        playerTyped = "";

        gameObject.SetActive(false);
    }

    private void OnNetworkSentenceChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        if (IsHost) return;

        randomWord = newValue.ToString();
        Debug.Log($"Updated randomWord on client: {randomWord}");

        SpawnWord();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetNetworkSentenceServerRpc()
    {
        PickRandomWord();

        Debug.Log($"Setting networkSentence on server: {randomWord}");
        networkSentence.Value = randomWord; 
    }

    public void SpawnWord()
    {
        ClearWords(); // Ensures no previous words exist
        ClearWordsOpponent();

        float baseLetterSpacing = Screen.width * 0.03f;
        float spaceSpacing = baseLetterSpacing * 1.5f;
        float yOffset = -50f; // Controls vertical offset of duplicate word

        List<float> letterPositions = new List<float>();
        float totalWidth = 0f;

        // Calculate total width first
        for (int i = 0; i < randomWord.Length; i++)
        {
            letterPositions.Add(totalWidth);
            totalWidth += (randomWord[i] == ' ') ? spaceSpacing : baseLetterSpacing;
        }

        // Center the sentence
        float startX = spawnPoint.position.x - (totalWidth / 2f);
        float originalY = spawnPoint.position.y;
        float duplicateY = originalY + yOffset;

        // Clear existing lists to prevent double spawns
        PrefabLettersInWord.Clear();
        charLettersInWord.Clear();
        PrefabLettersInWordOpponent.Clear();
        charLettersInWordOpponent.Clear();


        for (int i = 0; i < randomWord.Length; i++)
        {
            if (randomWord[i] == ' ') continue; // Skip spaces

            char letter = char.ToLower(randomWord[i]);
            Vector3 originalPosition = new Vector3(startX + letterPositions[i], originalY, spawnPoint.position.z);
            Vector3 duplicatePosition = new Vector3(startX + letterPositions[i], duplicateY, spawnPoint.position.z);

            // **Spawn original letter**
            GameObject newLetter = Instantiate(letterPrefab, originalPosition, Quaternion.identity, canvas.transform);
            if (letterDictionary.ContainsKey(letter))
            {
                newLetter.GetComponent<Image>().sprite = letterDictionary[letter];
            }
            PrefabLettersInWord.Add(newLetter);
            charLettersInWord.Add(letter);

            // **Spawn duplicate letter**
            GameObject duplicateLetter = Instantiate(letterPrefab, duplicatePosition, Quaternion.identity, canvas.transform);
            if (letterDictionary.ContainsKey(letter))
            {
                duplicateLetter.GetComponent<Image>().sprite = letterDictionary[letter];
            }
            PrefabLettersInWordOpponent.Add(duplicateLetter);
            charLettersInWordOpponent.Add(letter);

            // Ensure duplicate is non-interactable & slightly faded
            Image duplicateImage = duplicateLetter.GetComponent<Image>();
            duplicateImage.color = new Color(1f, 1f, 1f, 0.3f); // Set transparency
            duplicateImage.raycastTarget = false; // Make it non-interactable         
        }

        // **Ensure only one listener is attached**
        playerInput.onValueChanged.RemoveAllListeners();
        playerInput.onValueChanged.AddListener(delegate { CheckInput(); });
    }

    public void PickRandomWord()
    {
        ClearWords(); // Ensures no previous words exist
        ClearWordsOpponent();

        randomWord = wordsList[Random.Range(0, wordsList.Count)];

        float baseLetterSpacing = Screen.width * 0.03f;
        float spaceSpacing = baseLetterSpacing * 1.5f;
        float yOffset = -50f; // Controls vertical offset of duplicate word

        List<float> letterPositions = new List<float>();
        float totalWidth = 0f;

        // Calculate total width first
        for (int i = 0; i < randomWord.Length; i++)
        {
            letterPositions.Add(totalWidth);
            totalWidth += (randomWord[i] == ' ') ? spaceSpacing : baseLetterSpacing;
        }

        // Center the sentence
        float startX = spawnPoint.position.x - (totalWidth / 2f);
        float originalY = spawnPoint.position.y;
        float duplicateY = originalY + yOffset;

        // Clear existing lists to prevent double spawns
        PrefabLettersInWord.Clear();
        charLettersInWord.Clear();
        PrefabLettersInWordOpponent.Clear();
        charLettersInWordOpponent.Clear();


        for (int i = 0; i < randomWord.Length; i++)
        {
            if (randomWord[i] == ' ') continue; // Skip spaces

            char letter = char.ToLower(randomWord[i]);
            Vector3 originalPosition = new Vector3(startX + letterPositions[i], originalY, spawnPoint.position.z);
            Vector3 duplicatePosition = new Vector3(startX + letterPositions[i], duplicateY, spawnPoint.position.z);

            // **Spawn original letter**
            GameObject newLetter = Instantiate(letterPrefab, originalPosition, Quaternion.identity, canvas.transform);
            if (letterDictionary.ContainsKey(letter))
            {
                newLetter.GetComponent<Image>().sprite = letterDictionary[letter];
            }
            PrefabLettersInWord.Add(newLetter);
            charLettersInWord.Add(letter);

            // **Spawn duplicate letter**
            GameObject duplicateLetter = Instantiate(letterPrefab, duplicatePosition, Quaternion.identity, canvas.transform);
            if (letterDictionary.ContainsKey(letter))
            {
                duplicateLetter.GetComponent<Image>().sprite = letterDictionary[letter];
            }
            PrefabLettersInWordOpponent.Add(duplicateLetter);
            charLettersInWordOpponent.Add(letter);

            // Ensure duplicate is non-interactable & slightly faded
            Image duplicateImage = duplicateLetter.GetComponent<Image>();
            duplicateImage.color = new Color(1f, 1f, 1f, 0.3f); // Set transparency
            duplicateImage.raycastTarget = false; // Make it non-interactable         
        }

        // **Ensure only one listener is attached**
        playerInput.onValueChanged.RemoveAllListeners();
        playerInput.onValueChanged.AddListener(delegate { CheckInput(); });
    }


    void LoadWordsFromFile()
    {

        if (textAsset != null)
        {
            string[] words = textAsset.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            wordsList.AddRange(words);
           // Debug.Log("Loaded " + wordsList.Count + " words!");
        }
        else
        {
            Debug.LogError("Words file not found!");
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateOpponentWordColors();
        if (!GameManager.Instance.hasGameStarted.Value || !GameManager.Instance.isPlayer1Ready.Value || !GameManager.Instance.isPlayer2Ready.Value || GameManager.Instance.playerDied.Value) return;

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
                nrCorrectLetters = PrefabLettersInWord.Count;
                ChallengeManager.Instance.UpdatePlayerScoresServerRpc(NetworkManager.LocalClientId, nrCorrectLetters);
                ClearWords();
                ClearWordsOpponent();
                CinematicManager.Instance.StopCinematic();

             
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
    public void ClearWordsOpponent()
    {
        if (charLettersInWordOpponent.Count > 0)
        {
            charLettersInWordOpponent.Clear();
        }
        if (PrefabLettersInWordOpponent.Count > 0)
        {
            for (int i = 0; i < PrefabLettersInWordOpponent.Count; i++)
            {
                PrefabLettersInWordOpponent[i].gameObject.SetActive(false);
            }
            PrefabLettersInWordOpponent.Clear();
        }
    }
    private int nrCorrectLetters = 1;
    void CheckInput()
    {
        playerTyped = playerInput.text.ToUpper();
        int newCorrectLetters = 0; // Local variable to track correct letters in this input

        for (int i = 0; i < PrefabLettersInWord.Count; i++)
        {
            Image letterImage = PrefabLettersInWord[i].GetComponent<Image>();

            if (i < playerTyped.Length)
            {
                if (char.ToLower(playerTyped[i]) == charLettersInWord[i])
                {
                    letterImage.color = Color.green; // Correct letter
                    newCorrectLetters++;
                }
                else
                {
                    letterImage.color = Color.red; // Incorrect letter
                }
            }
            else
            {
                letterImage.color = Color.white; // Reset color for remaining letters
            }
        }

        // Update nrCorrectLetters only once per input
        nrCorrectLetters = newCorrectLetters;

        ChallengeManager.Instance.UpdatePlayerScoresServerRpc(NetworkManager.LocalClientId, nrCorrectLetters);
    }


    private void UpdateOpponentWordColors()
    {
        float opponentProgress = (NetworkManager.LocalClientId == 0)
            ? ChallengeManager.Instance.player2ProgressionInChallenge.Value
            : ChallengeManager.Instance.player1ProgressionInChallenge.Value;

        for (int i = 0; i < PrefabLettersInWordOpponent.Count; i++)
        {          
            // If the character is within the opponent's progress, make it green
            if (i < opponentProgress)
            {
                PrefabLettersInWordOpponent[i].GetComponent<Image>().color = Color.green;
                if (opponentProgress >= PrefabLettersInWordOpponent.Count)
                {
                    ClearWordsOpponent();
                }
            }
            // Otherwise, make it white
            else
            {
                PrefabLettersInWordOpponent[i].GetComponent<Image>().color = Color.white;
            }
        }
       
    }


}

