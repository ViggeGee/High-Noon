using System.Collections.Generic;
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

    void Start()
    {
        LoadWordsFromFile();
        // Populate dictionary (Assumes prefab names are "A", "B", "C", etc.)
        foreach (Sprite texture in letterextures)
        {
            char letter = texture.name[0]; // Get first character from prefab name
            letterDictionary[letter] = texture;
        }

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

        playerInput.Select();
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
        playerInput.Select();

    }

    void CheckInput()
    {
        string playerTyped = playerInput.text.ToUpper();

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
                }
            }
            else
            {
                letterImage.color = Color.white; // Reset color for remaining letters
            }
        }

        // Check if player finished the word
        if (playerTyped == randomWord)
        {
            Debug.Log("Correct! Word completed: " + randomWord);
            playerInput.text = ""; // Clear input for next word
            Start(); // Restart with a new word
        }
    
    }

 

}
