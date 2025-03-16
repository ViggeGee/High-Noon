using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSorce;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    public static void PlayLoopedMusic(AudioClip song)
    {
        Instance.musicSorce.clip = song;
        Instance.musicSorce.Play();
    }

    public static void PlaySFX(AudioClip sfx)
    {
        Instance.sfxSource.clip = sfx;
        Instance.sfxSource.Play();
    }

    public static void PlayLoopedAmbient(AudioClip ambient)
    {
        Instance.ambientSource.clip = ambient;
        Instance.ambientSource.Play();
    }

    public static void StopMusic()
    {
        Instance.musicSorce.Stop();
    }

    public static void StopAmbient()
    {
        Instance.ambientSource.Stop();
    }

    public static void PlayRandomSFX(SFX sfx)
    {
        int randomIndex = Random.Range(0, sfx.audioClips.Length);
        PlaySFX(sfx.audioClips[randomIndex]);
    }   

}

