using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

namespace VA_Controls
{
    public class VoiceControl : MonoBehaviour
    {
        private KeywordRecognizer keywordRecognizer;
        private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

        private bool isGrounded = true;

        public AudioSource audioSource;
        public float yellThreshold = 0.01f; // Adjust sensitivity
        private const int sampleDataLength = 1024;
        private float[] sampleData;

        void Start()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA
            //Voice Recognition Detection
            keywords.Add("MOVE", MoveBirds);

            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            keywordRecognizer.Start();

            // Yell Detection
            sampleData = new float[sampleDataLength];
            StartMicrophone();
#else
            Debug.LogWarning("Speech recognition is not supported on this platform.");
#endif
        }
        private void StartMicrophone()
        {
            // Get all available microphones
            string[] availableMics = Microphone.devices;
            Debug.Log("Available Microphones: " + string.Join(", ", availableMics));

            if (availableMics.Length == 0)
            {
                Debug.LogError("No microphones detected!");
                return;
            }

            // Assign a microphone based on player name or other unique identifier
            string micDevice;
            if (gameObject.name == "Player1" && availableMics.Length > 0)
            {
                micDevice = availableMics[0]; // Assign first mic to Player 1
            }
            else if (gameObject.name == "Player2" && availableMics.Length > 1)
            {
                micDevice = availableMics[1]; // Assign second mic to Player 2
            }
            else
            {
                micDevice = availableMics[0]; // Default to first mic if only one is found
            }

            Debug.Log($"[{gameObject.name}] Using Microphone: {micDevice}");

            // Start recording with the assigned microphone
            audioSource.clip = Microphone.Start(micDevice, true, 10, 44100);
            audioSource.loop = true;
            audioSource.mute = true;

            while (!(Microphone.GetPosition(micDevice) > 0)) { } // Wait for mic to start
            audioSource.Play();
        }



        void Update()
        {
            DetectYell();
            Debug.Log("Mic Loudness: " + CalculateLoudness(sampleData));
        }


        private void DetectYell()
        {
            if (audioSource.clip == null) return;

            audioSource.clip.GetData(sampleData, audioSource.timeSamples);
            float loudness = CalculateLoudness(sampleData);

            Debug.Log("Current Loudness: " + loudness + "/" + yellThreshold);

            if (loudness >= yellThreshold)
            {
                Debug.LogWarning("YELL DETECTED! Triggering MoveBirds()");
                MoveBirds(); // Call your action
            }
        }

        private float CalculateLoudness(float[] data)
        {
            float sum = 0f;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i] * data[i]; // Square the sample values
            }
            float rmsValue = Mathf.Sqrt(sum / data.Length); // Root Mean Square (RMS)

            return rmsValue * 10; // Scale up the loudness
        }

        private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            Debug.Log("Recognized: " + args.text);
            if (keywords.ContainsKey(args.text))
            {
                keywords[args.text].Invoke();
            }
        }

        private void MoveBirds()
        {

            GetComponent<TriggerBird>().triggerBirds = true;

        }

        private void OnDestroy()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
        }
    }
}
