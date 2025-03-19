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
            keywords.Add("Move Forward", PerformMoveForward);
            keywords.Add("Move Backward", PerformMoveBackward);
            keywords.Add("Move Left", PerformMoveLeft);
            keywords.Add("Move Right", PerformMoveRight);

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

        private void PerformMoveForward()
        {

            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(Vector3.forward * 2, ForceMode.Impulse);
                Debug.Log("Move Forward action executed.");
                
            }
        }

        private void PerformMoveBackward()
        {
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(Vector3.back * 2, ForceMode.Impulse);
                Debug.Log("Move Backward action executed.");
            }
        }

        private void PerformMoveLeft()
        {
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(Vector3.left * 2, ForceMode.Impulse);
                Debug.Log("Move Left action executed.");
            }
        }

        private void PerformMoveRight()
        {
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(Vector3.right * 2, ForceMode.Impulse);
                Debug.Log("Move Right action executed.");
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
