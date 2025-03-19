using System;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SpinManager : NetworkBehaviour
{
    [SerializeField] private Image[] buttons;
    [SerializeField] private SpinKey[] keys;
    [SerializeField] private Image emptyBar;
    [SerializeField] private Image emptyBarOpponent;
    [SerializeField] private Image fillBar;
    [SerializeField] private Image fillBarOpponent;
    [SerializeField] private TextMeshProUGUI tmp_instructions;
    [SerializeField] private bool isSinglePlayer;

    public int nextButtonIndex = 0;
    public bool challengeCompleted = false;

    void Update()
    {

        if (NetworkManager.LocalClientId == 0)
        {
            fillBarOpponent.fillAmount = ChallengeManager.Instance.player2ProgressionInChallenge.Value;
        }
        else
        {
            fillBarOpponent.fillAmount = ChallengeManager.Instance.player1ProgressionInChallenge.Value;
        }

        if (fillBarOpponent.fillAmount >= 1)
        {
            fillBarOpponent.gameObject.SetActive(false);
            emptyBarOpponent.gameObject.SetActive(false);
        }

        CanvasSettings();
        SetColor();

        if (!challengeCompleted)
        {
            ChallengeManager.Instance.UpdatePlayerScoresServerRpc(NetworkManager.LocalClientId, fillBar.fillAmount);
            SpinActivated();
        }
        else
        {
            ChallengeManager.Instance.UpdatePlayerScoresServerRpc(NetworkManager.LocalClientId, 1);
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
            fillBar.gameObject.SetActive(false);
            emptyBar.gameObject.SetActive(false);
            emptyBarOpponent.gameObject.SetActive(false);
            fillBarOpponent.gameObject.SetActive(false);
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
            fillBar.gameObject.SetActive(true);
            emptyBar.gameObject.SetActive(true);
            foreach (var item in buttons)
            {
                item.gameObject.SetActive(true);
            }
            tmp_instructions.gameObject.SetActive(true);
        }
    }

    private void SpinActivated()
    {
        if (!GameManager.Instance.hasGameStarted.Value || !GameManager.Instance.isPlayer1Ready.Value || !GameManager.Instance.isPlayer2Ready.Value || GameManager.Instance.playerDied.Value) return;


        bool buttonPressed = keys[nextButtonIndex].IsKeyDown();

        fillBar.fillAmount -= 0.05f * Time.deltaTime;

        if (buttonPressed)
        {
            nextButtonIndex++;
            if (nextButtonIndex >= buttons.Length)
                nextButtonIndex = 0;

            fillBar.fillAmount += 2f * Time.deltaTime;

            if (fillBar.fillAmount >= 1)
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
