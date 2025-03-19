using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinManager : MonoBehaviour
{
    [SerializeField] private Image[] buttons;
    [SerializeField] private SpinKey[] keys;
    [SerializeField] private Image emptyBar;
    [SerializeField] private Image loadingBar;
    [SerializeField] private TextMeshProUGUI tmp_instructions;
    [SerializeField] private int timer = 5;
    [SerializeField] private bool isSinglePlayer;

    public int nextButtonIndex = 0;
    public bool challengeCompleted = false;

    void Update()
    {

        CanvasSettings();
        SetColor();

        if (!challengeCompleted)
        {
            SpinActivated();
        }

    }

    private void SetColor()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == nextButtonIndex)
                buttons[i].color = Color.white;
            else
                buttons[i].color = Color.gray;
        }
    }

    private void CanvasSettings()
    {
        //loadingBar.rectTransform.sizeDelta = new Vector2(barWidth, loadingBar.rectTransform.sizeDelta.y);
        if (challengeCompleted)
        {
            loadingBar.gameObject.SetActive(false);
            foreach (var item in buttons)
            {
                item.gameObject.SetActive(false);
            }

            if (challengeCompleted)
            {
                tmp_instructions.gameObject.SetActive(false);
            }
        }
        else
        {
            loadingBar.gameObject.SetActive(true);
            foreach (var item in buttons)
            {
                item.gameObject.SetActive(true);
            }
            tmp_instructions.gameObject.SetActive(true);
        }
    }

    private void SpinActivated()
    {
        if (!isSinglePlayer)
        {
            if (!GameManager.Instance.hasGameStarted.Value ||
                !GameManager.Instance.isPlayer1Ready.Value ||
                !GameManager.Instance.isPlayer2Ready.Value ||
                GameManager.Instance.playerDied.Value)
                return;
        }

        bool buttonPressed = keys[nextButtonIndex].IsKeyDown();

        loadingBar.fillAmount -= 0.05f * Time.deltaTime;

        if (buttonPressed)
        {
            nextButtonIndex++;
            if (nextButtonIndex >= buttons.Length)
                nextButtonIndex = 0;

            loadingBar.fillAmount += 1f * Time.deltaTime;

            if (loadingBar.fillAmount >= 1)
            {
                challengeCompleted = true;
                GameManager.Instance.readyToShoot = true;
                CinematicManager.Instance.StopCinematic();
            }
        }
    }
}

[Serializable]
public class SpinKey
{
    [SerializeField] private KeyCode[] Keys;
    public KeyCode[] keys => Keys;

    public bool IsKeyDown()
    {
        foreach (var item in Keys)
        {
            if (Input.GetKeyDown(item))
            {
                return true;
            }
        }
        return false;
    }
}
