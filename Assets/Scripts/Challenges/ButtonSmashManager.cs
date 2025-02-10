using System.Runtime.CompilerServices;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSmashManager : MonoBehaviour
{
    [SerializeField] private Image button1;
    [SerializeField] private Image button2;
    [SerializeField] private float maximumButtonPresses;
    [SerializeField] private float currentButtonPresses = 0;
    [SerializeField] private TextMeshProUGUI tmp_instructions;


    private StarterAssetsInputs input;
    public bool nextIsButton1 = true;
    public bool challengeCompleted = false;


    void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {

        ButtonSmashActivated();
        CanvasSettings();
        SetColor();

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
        if (challengeCompleted)
        {
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            tmp_instructions.gameObject.SetActive(false);
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

        if (currentButtonPresses > maximumButtonPresses)
        {
            challengeCompleted = true;
        }
    }
}
