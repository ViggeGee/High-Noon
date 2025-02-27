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

        public Rigidbody playerRigidbody;
        public float jumpForce = 5f;
        private bool isGrounded = true;

        void Start()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA
            // Define keyword and corresponding action for jump only
            keywords.Add("jump", PerformJump);

            // Initialize the KeywordRecognizer with the keywords
            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            keywordRecognizer.Start();
#else
            Debug.LogWarning("Speech recognition is not supported on this platform.");
#endif
        }

        private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            Debug.Log("Recognized: " + args.text);
            if (keywords.ContainsKey(args.text))
            {
                keywords[args.text].Invoke();
            }
        }

        private void PerformJump()
        {
            if (isGrounded && playerRigidbody != null)
            {
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                Debug.Log("Jump action executed.");
                isGrounded = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
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
