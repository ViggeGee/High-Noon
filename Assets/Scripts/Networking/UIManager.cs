using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Image winImage, looseImage;
    public TextMeshProUGUI scoreLooseScreen, scoreWinScreen;

    public NetworkVariable<bool> countdownStarted = new NetworkVariable<bool>(false);

    public Canvas GetGameOverCanvas { get { return gameOverCanvas; } }
    public Image GetWinImage { get { return winImage; } }
    public Image GetLooseImage { get { return looseImage; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        countdownStarted.OnValueChanged += (oldValue, newValue) =>
        {
            if (newValue && ChallengeManager.Instance.currentChallengeType.Value != Challenge.ChallengeType.None)
            {
                StartCoroutine(StartCountdown());

                // Once countdown starts, trigger the countdown function
            }
        };
    }
    public void ShowGameOverScreen(bool playerWon)
    {
        gameOverCanvas.gameObject.SetActive(true);
        winImage.gameObject.SetActive(playerWon);
        looseImage.gameObject.SetActive(!playerWon);
    }

    public IEnumerator StartCountdown()
    {
        yield return FadeText("3");
        yield return FadeText("2");
        yield return FadeText("1");
        yield return FadeText("DRAW");

        countDownText.text = "";
        
        if(IsServer)
        {
            GameManager.Instance.hasGameStarted.Value = true;
        }    

        ulong thisClient = NetworkManager.Singleton.LocalClientId;

        SetPlayerReadyServerRpc(thisClient);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ulong localClientId)
    {
        if (localClientId == 0)
        {
            GameManager.Instance.isPlayer1Ready.Value = true;
        }
        else
        {
            GameManager.Instance.isPlayer2Ready.Value = true;
        }
    }

    IEnumerator FadeText(string newText)
    {
        // Scale down
        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(1, 0, t / 0.2f);
            countDownText.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        countDownText.transform.localScale = Vector3.zero;

        // Change the text
        countDownText.text = newText;

        // Scale back up
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(0, 1, t / 0.3f);
            countDownText.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        countDownText.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(1.5f); // Stay visible before next fade
    }

    private void StartCountDown()
    {
        countdownStarted.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartCountDownServerRpc()
    {
        StartCountDown();  // Call the original function on the server
    }
}

