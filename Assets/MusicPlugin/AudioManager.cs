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
    public AudioClip westernTownMusic;
    public AudioClip trainMapMusic;
    public AudioClip trainHorseMapMusic;
    public AudioClip bisonMusic;
    public AudioClip saloonMusic;
    public AudioClip mountainMusic;

    [Header("Sound Effects")]
    public SFX deathSFX;
    public SFX hitSFX;

    [Header("Specific Scene Ambiance")]
    public AudioClip mainMenuAmbiance;
    [Range(0, 1)]
    public float mainMenuAmbianceVolume = 1;

    public AudioClip westernTownAmbiance;
    [Range(0, 1)] public float westernTownAmbianceVolume = 1;

    public AudioClip bisonAmbiance;
    [Range(0, 1)] public float bisonAmbianceVolume = 1;

    public AudioClip trainMapAmbiance;
    [Range(0, 1)] public float trainMapAmbianceVolume = 1;

    public AudioClip gunshotLoopSaloon;
    [Range(0, 1)] public float gunshotLoopSaloonVolume = 1;

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
        StartSceneSounds();

        //play predefined ambiance for each scene or dont play anything at all
    }

    public void NewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetAudio();
    }

    public void StartSceneSounds()
    {
        ambientSource.volume = ambientVolume;

        switch(SceneManager.GetActiveScene().name)
        {
            //NON GAMEPLAY SCENES
            case "StartGameScene":
                break;
            case "MainMenu":
                PlayLoopedMusic(mainMenuMusic);
                break;
            case "JoinGameScene":
                PlayLoopedMusic(mainMenuMusic);
                break;


            //GAMEPLAY SCENES
            case "Bison":
                PlayLoopedMusic(randomSong);
                PlayLoopedAmbient(bisonMusic);

                break;
            case "WesternTown":
                PlayLoopedMusic(randomSong);
                PlayLoopedAmbient(bisonMusic);
                break;
            case "TrainMap":
                PlayLoopedMusic(trainMapMusic);

                ambientSource.volume = trainMapAmbianceVolume;
                PlayLoopedAmbient(trainMapAmbiance);
                break; 
            case "Saloon":
                PlayLoopedMusic(saloonMusic);
                PlayLoopedAmbient(gunshotLoopSaloon);
                break;


            //CHOOSES A RANDOM ONE IF NONE IS DECIDED
            default:
                PlayLoopedMusic(randomSong);
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

