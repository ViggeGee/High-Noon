using UnityEngine;

public class ShootingGallerySFX : MonoBehaviour
{
    public static ShootingGallerySFX Instance; // Singleton instance

    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip airGunShoot; // Sound for left clicking on screen
    public AudioClip hitTarget;
    public AudioClip[] screams; // Sound for hitting a button



    private void Awake()
    {
        // Singleton pattern for easy access
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Play the left click sound effect
    public void PlayLeftClick()
    {
        if (airGunShoot != null)
        {
            audioSource.PlayOneShot(airGunShoot);
        }
    }

    // Play the hit button sound effect
    public void PlayHitTarget()
    {
        if (hitTarget != null)
        {
            audioSource.PlayOneShot(hitTarget);
        }
    }

    public void PlayRandomScream()
    {
        int randomIndex = Random.Range(0, screams.Length);

        if (screams[randomIndex] != null)
        {
            audioSource.PlayOneShot(screams[randomIndex]);
        }
    }
}
