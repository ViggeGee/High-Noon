using System.Runtime.CompilerServices;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSmashManager : MonoBehaviour
{
    //[SerializeField] private Image button1;
    //[SerializeField] private Image button2;
    [SerializeField] private float maximumButtonPresses;
    [SerializeField] private float currentButtonPresses = 0;
    [SerializeField] private float button1Value;
    [SerializeField] private float button2Value;
    [SerializeField] private bool button1Active = false;
    [SerializeField] private bool button2Active = false;
    private StarterAssetsInputs input;
    public bool nextIsButton1 = true;



    void Start()
    {
        button1Active = false;
        button2Active = false;
        input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        ButtonSmashActivated();
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
    }
}
