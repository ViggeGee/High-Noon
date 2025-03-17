using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    public SFX randomSong;

    [Header("Scene Music")]
    public AudioClip mainMenuMusic;

    [Header("Scene Ambiance")]
    public AudioClip mainMenuAmbiance;

    private void Awake()
    {
        musicSource.Stop();
        sfxSource.Stop();
        ambientSource.Stop();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //play predefined music for each scene, or choose random song
        StartSceneMusic();

        //play predefined ambiance for each scene or dont play anything at all
        StartSceneAmbiance();
    }

    public void StartSceneMusic()
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "MainMenu":
                PlayLoopedMusic(mainMenuMusic);
                break;
            default:
                PlayLoopedMusic(randomSong);
                break;
        }
    }

    public void StartSceneAmbiance()
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "MainMenu":
                PlayLoopedAmbient(mainMenuAmbiance);
                break;
            default:
                break;
        }
    }
    public void PlayLoopedMusic(AudioClip song)
    {
        Instance.musicSource.clip = song;
        Instance.musicSource.Play();
    }

    public void PlayLoopedMusic(SFX song)
    {
        int randomIndex = Random.Range(0, song.audioClips.Length);
        PlayLoopedMusic(song.audioClips[randomIndex]);
    }

    public void PlaySFX(AudioClip sfx)
    {
        Instance.sfxSource.clip = sfx;
        Instance.sfxSource.Play();
    }

    public void PlayLoopedAmbient(AudioClip ambient)
    {
        Instance.ambientSource.clip = ambient;
        Instance.ambientSource.Play();
    }

    public void StopMusic()
    {
        Instance.musicSource.Stop();
    }

    public void StopAmbient()
    {
        Instance.ambientSource.Stop();
    }

    public void PlayRandomSFX(SFX sfx)
    {
        int randomIndex = Random.Range(0, sfx.audioClips.Length);
        PlaySFX(sfx.audioClips[randomIndex]);
    }   

}

