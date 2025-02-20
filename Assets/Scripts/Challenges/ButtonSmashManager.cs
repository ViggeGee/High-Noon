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
    [SerializeField] private TextMeshProUGUI tmp_instructions;
    [SerializeField] private int timer = 5;

    public bool nextIsButton1 = true;
    public bool challengeCompleted = false;
    //private bool challengeStarted = false;
    private bool canPress = true;


    // Update is called once per frame
    void Update()
    {

        CanvasSettings();
        SetColor();

        if (!challengeCompleted)
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
        //loadingBar.rectTransform.sizeDelta = new Vector2(barWidth, loadingBar.rectTransform.sizeDelta.y);
        if (challengeCompleted)
        {
            loadingBar.gameObject.SetActive(false);
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
            button1.gameObject.SetActive(true);
            button2.gameObject.SetActive(true);
            tmp_instructions.gameObject.SetActive(true);
        }
    }

    private void ButtonSmashActivated()
    {
        if (!GameManager.Instance.hasGameStarted.Value || !GameManager.Instance.isPlayer1Ready.Value || !GameManager.Instance.isPlayer2Ready.Value || GameManager.Instance.playerDied.Value) return;


        bool pressedButton1 = Input.GetKeyDown(KeyCode.Q) && nextIsButton1;
        bool pressedButton2 = Input.GetKeyDown(KeyCode.E) && !nextIsButton1;

        loadingBar.fillAmount -= 0.05f * Time.deltaTime;

        if (pressedButton1 || pressedButton2)
        {

            if (pressedButton1)
            {
                nextIsButton1 = false;
                loadingBar.fillAmount += 1f * Time.deltaTime;
            }
            else
            {
                nextIsButton1 = true;
                loadingBar.fillAmount += 1f * Time.deltaTime;
            }

            if (loadingBar.fillAmount >= 1)
            {
                challengeCompleted = true;
                GameManager.Instance.readyToShoot = true;
                CinematicManager.Instance.StopCinematic();
            }
        }
    }
}
