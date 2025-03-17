using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Volume Settings, only change on startup")]
    [Range(0, 1)]
    public float musicVolume = 0.5f;

    [Range(0, 1)]
    public float sfxVolume = 0.5f;

    [Range(0, 1)]
    public float ambientVolume = 0.5f;

    [Header("Specific Scene Music")]
    public AudioClip mainMenuMusic;
    public AudioClip westernTown;
    public AudioClip TrainMap;
    public AudioClip TrainHorseMap;
    public AudioClip Bison;
    public AudioClip Saloon;
    public AudioClip Mountain;

    [Header("Sound Effects")]
    public SFX deathSFX;
    public SFX hitSFX;

    [Header("Specific Scene Ambiance")]
    public AudioClip mainMenuAmbiance;

    [Header("Randomized")]
    public SFX randomSong;

    private void Awake()
    {
        SceneManager.sceneLoaded += NewSceneLoaded;

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        ambientSource.volume = ambientVolume;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        ResetAudio();
    }

    public void ResetAudio()
    {
        musicSource.Stop();
        sfxSource.Stop();
        ambientSource.Stop();

        //play predefined music for each scene, or choose random song
        StartSceneMusic();

        //play predefined ambiance for each scene or dont play anything at all
        StartSceneAmbiance();
    }

    public void NewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetAudio();
    }

    public void StartSceneMusic()
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "StartGameScene":
                break;
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
    public void PlaySFX(string sfx)
    {
        switch (sfx)
        {
            case "Death":
                PlayRandomSFX(deathSFX);
                break;
            case "Hit":
                PlayRandomSFX(hitSFX);
                break;
        }
            
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

