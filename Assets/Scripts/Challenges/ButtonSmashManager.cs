using System.Collections;
using System.Runtime.CompilerServices;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSmashManager : MonoBehaviour
{
    [SerializeField] private Image button1;
    [SerializeField] private Image button2;
    [SerializeField] private TextMeshProUGUI countdown;
    [SerializeField] private float maximumButtonPresses;
    [SerializeField] private float currentButtonPresses = 0;
    [SerializeField] private TextMeshProUGUI tmp_instructions;
    [SerializeField] private Transform player;
    [SerializeField] private int timer = 5;

    private StarterAssetsInputs input;
    public bool nextIsButton1 = true;
    public bool challengeCompleted = false;
    private bool challengeStarted = false;


    void Start()
    {
        input = player.GetComponent<StarterAssetsInputs>();
        StartCoroutine(CountdownRoutine());
    }

    // Update is called once per frame
    void Update()
    {

        CanvasSettings();
        SetColor();

        if (challengeStarted && !challengeCompleted)
        {
            ButtonSmashActivated();
        }

    }

    private void SetColor()
    {
        if (nextIsButton1)
        {
            button1.color = Color.white;
            button2.color = Color.gray;
        }
        else
        {
            button1.color = Color.gray;
            button2.color = Color.white;
        }
    }

    private void CanvasSettings()
    {
        if (challengeCompleted || !challengeStarted)
        {
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            tmp_instructions.gameObject.SetActive(false);
        }
        else
        {
            button1.gameObject.SetActive(true);
            button2.gameObject.SetActive(true);
            tmp_instructions.gameObject.SetActive(true);
        }
    }

    private void ButtonSmashActivated()
    {
        if (input.buttonSmash1 && nextIsButton1)
        {
            input.buttonSmash1 = false;
            nextIsButton1 = false;
            currentButtonPresses++;
        }
        else if (input.buttonSmash2 && !nextIsButton1)
        {
            input.buttonSmash2 = false;
            nextIsButton1 = true;
            currentButtonPresses++;
        }

        if (currentButtonPresses >= maximumButtonPresses)
        {
            challengeCompleted = true;
        }
    }

    IEnumerator CountdownRoutine()
    {
        int timeLeft = timer;

        while (timeLeft > 0)
        {
            countdown.text = timeLeft.ToString(); // Update UI
            yield return new WaitForSeconds(1f); // Wait 1 second
            timeLeft--; // Decrease time
        }
        challengeStarted = true;
        countdown.text = "GO!"; // Display final message
        yield return new WaitForSeconds(1f);
        countdown.text = ""; // Clear text after 1 sec (optional)
    }
}
