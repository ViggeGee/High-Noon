using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechTest : MonoBehaviour
{
    void Start()
    {

        Debug.Log("PhraseRecognitionSystem Status BEFORE: " + PhraseRecognitionSystem.Status);

        if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Stopped)
        {
            Debug.Log("Trying to start PhraseRecognitionSystem...");
            PhraseRecognitionSystem.Restart();  // Manually restart it
        }

        Debug.Log("PhraseRecognitionSystem Status AFTER: " + PhraseRecognitionSystem.Status);
    

    //Debug.Log("PhraseRecognitionSystem Status: " + PhraseRecognitionSystem.Status);

    //PhraseRecognitionSystem.OnError += (e) =>
    //{
    //    Debug.LogError("Speech Recognition Error: " + e);
    //};

    //PhraseRecognitionSystem.OnStatusChanged += (status) =>
    //{
    //    Debug.Log("Speech Recognition Status Changed: " + status);
    //};

    //foreach (var device in Microphone.devices)
    //{
    //    Debug.Log("Detected microphone: " + device);
    //}
}
}
