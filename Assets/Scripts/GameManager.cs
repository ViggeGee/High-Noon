using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// This script handles the game loop 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region UI variables

    [SerializeField] private TextMeshProUGUI countDownText;

    #endregion

    #region Challenge

    public Challenge.ChallengeType currentChallengeType;

    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static bool bHasGameStarted = false;



    #region Countdown and challenge selection

    //private IEnumerator CountDown()
    //{
    //    yield return StartCoroutine(FadeText("3"));
    //    yield return StartCoroutine(FadeText("2"));
    //    yield return StartCoroutine(FadeText("1"));
    //    yield return StartCoroutine(FadeText("DRAW"));

    //    canvasText.text = "";
    //    startGameSound.Play();
    //    gameStarted = true;
    //}
    //IEnumerator FadeText(string newText)
    //{
    //    // Scale down
    //    for (float t = 0; t < 0.2f; t += Time.deltaTime)
    //    {
    //        float scale = Mathf.Lerp(1, 0, t / 0.2f);
    //        canvasText.transform.localScale = new Vector3(scale, scale, scale);
    //        yield return null;
    //    }
    //    canvasText.transform.localScale = Vector3.zero;

    //    // Change the text
    //    canvasText.text = newText;

    //    // Scale back up
    //    for (float t = 0; t < 0.3f; t += Time.deltaTime)
    //    {
    //        float scale = Mathf.Lerp(0, 1, t / 0.3f);
    //        canvasText.transform.localScale = new Vector3(scale, scale, scale);
    //        yield return null;
    //    }
    //    canvasText.transform.localScale = Vector3.one;

    //    yield return new WaitForSeconds(1.5f); // Stay visible before next fade
    //}

    #endregion

}
