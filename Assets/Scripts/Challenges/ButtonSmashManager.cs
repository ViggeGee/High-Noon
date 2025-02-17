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
    [SerializeField] private Image emptyBar;
    [SerializeField] private Image loadingBar;
    [SerializeField] private TextMeshProUGUI countdown;
    [SerializeField] private float maximumButtonPresses;
    [SerializeField] private float currentButtonPresses = 0;
    [SerializeField] private TextMeshProUGUI tmp_instructions;
    [SerializeField] private Transform player;
    [SerializeField] private int timer = 5;
    private float barWidth = 0;

    private StarterAssetsInputs input;
    public bool nextIsButton1 = true;
    public bool challengeCompleted = false;
    private bool challengeStarted = false;
    private bool canPress = true;


    void Start()
    {
        input = player.GetComponent<StarterAssetsInputs>();
        StartCoroutine(CountdownRoutine());
        loadingBar.rectTransform.sizeDelta = new Vector2(barWidth, loadingBar.rectTransform.sizeDelta.y);
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
        loadingBar.rectTransform.sizeDelta = new Vector2(barWidth, loadingBar.rectTransform.sizeDelta.y);
        if (challengeCompleted || !challengeStarted)
        {
            loadingBar.gameObject.SetActive(false);
            emptyBar.gameObject.SetActive(false);
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);

            if (challengeCompleted)
            {
                tmp_instructions.gameObject.SetActive(false);
            }
        }
        else
        {
            loadingBar.gameObject.SetActive(true);
            emptyBar.gameObject.SetActive(true);
            button1.gameObject.SetActive(true);
            button2.gameObject.SetActive(true);
            tmp_instructions.gameObject.SetActive(true);
        }
    }

    private void ButtonSmashActivated()
    {
        // If we're in the middle of the short delay, do nothing
        if (!canPress) return;

        bool pressedButton1 = input.buttonSmash1 && nextIsButton1;
        bool pressedButton2 = input.buttonSmash2 && !nextIsButton1;

        if (pressedButton1 || pressedButton2)
        {
            StartCoroutine(ButtonPressDelay());

            if (pressedButton1)
            {
                input.buttonSmash1 = false;
                nextIsButton1 = false;
                barWidth += 1.95f;
            }
            else
            {
                input.buttonSmash2 = false;
                nextIsButton1 = true;
                barWidth += 1.95f;
            }

            currentButtonPresses++;

            if (currentButtonPresses >= maximumButtonPresses)
            {
                challengeCompleted = true;
            }
        }
    }

    IEnumerator CountdownRoutine()
    {
        int timeLeft = timer;

        while (timeLeft > 0)
        {
            countdown.text = timeLeft.ToString();
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        challengeStarted = true;
        countdown.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdown.text = "";
    }

    
    private IEnumerator ButtonPressDelay()
    {
        canPress = false;
        yield return new WaitForSeconds(0.05f);
        canPress = true;
    }
}
