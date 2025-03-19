using SimpleAudioManager;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    #region Variables
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Volume Settings, only change on startup")]
    [Range(0, 1)]
    public float masterVolume = 0.5f;

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
    public SFX screamSFX;

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

    #endregion

    #region On Game Startup
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

    #endregion

    #region Loading New Scene

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
                PlayLoopedAmbient(bisonAmbiance);

                break;
            case "WesternTown":
                PlayLoopedMusic(randomSong);
                break;        
            case "Bird":
                PlayLoopedMusic(randomSong);
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
            case "TrainHorseMap":
                PlayLoopedMusic(trainMapMusic);

                ambientSource.volume = trainMapAmbianceVolume;
                PlayLoopedAmbient(trainMapAmbiance);
                break;

            //CHOOSES A RANDOM ONE IF NONE IS DECIDED
            default:
                PlayLoopedMusic(randomSong);
                break;
        }
    }
    #endregion

    #region Play Music
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

    #endregion

    #region PlaySFX
    public void PlaySFX(AudioClip sfx)
    {
        Instance.sfxSource.PlayOneShot(sfx);
    }

    public void PlaySFX(string sfx)
    {
        switch (sfx)
        {
            case "Death":
                PlaySFX(deathSFX);
                break;
            case "Hit":
                PlaySFX(hitSFX);
                break;
            case "Scream":
                PlaySFX(screamSFX);
                break;
        }
            
    }
    public void PlaySFX(SFX sfx)
    {
        int randomIndex = Random.Range(0, sfx.audioClips.Length); 
        PlaySFX(sfx.audioClips[randomIndex]);
    }
    #endregion

    #region SFX At Location
    public void PlaySFXAtLocation(AudioClip sfx, Vector3 position, float volume)
    {
        AudioSource.PlayClipAtPoint(sfx, position, volume);
    }
    public void PlaySFXAtLocation(SFX sfx, Vector3 position, float volume)
    {
        int randomIndex = Random.Range(0, sfx.audioClips.Length);
        AudioSource.PlayClipAtPoint(sfx.audioClips[randomIndex], position, volume);
    }  
    public void PlaySFXAtLocation(AudioClip sfx, Transform transform, float volume)
    {
        AudioSource.PlayClipAtPoint(sfx, transform.position, volume);
    }

    #endregion

    #region Play Ambiance
    public void PlayLoopedAmbient(AudioClip ambient)
    {
        Instance.ambientSource.clip = ambient;
        Instance.ambientSource.Play();
    }
    #endregion

    #region Audio Control
    public void ResetAudio()
    {
        musicSource.Stop();
        sfxSource.Stop();
        ambientSource.Stop();

        //play predefined music for each scene, or choose random song
        StartSceneSounds();

        //play predefined ambiance for each scene or dont play anything at all
    }
    public void StopMusic()
    {
        Instance.musicSource.Stop();
    }
    public void StopAmbient()
    {
        Instance.ambientSource.Stop();
    }

    #endregion

    #region Volume Control
    public void SetVolume(float masterVolume, float musicVolume, float sfxVolume, float ambianceVolume)
    {
        Instance.masterVolume = masterVolume;
        Instance.musicVolume = musicVolume;
        Instance.sfxVolume = sfxVolume;
        Instance.ambientVolume = ambianceVolume;

        Instance.musicSource.volume = Instance.musicVolume * Instance.masterVolume;
        Instance.sfxSource.volume = Instance.sfxVolume * Instance.masterVolume;
        Instance.ambientSource.volume = Instance.ambientVolume * Instance.masterVolume;
    }

    public void SetMasterVolume(float masterVolume)
    {
        Instance.masterVolume = masterVolume;
        Instance.musicSource.volume = Instance.musicVolume * Instance.masterVolume;
        Instance.sfxSource.volume = Instance.sfxVolume * Instance.masterVolume;
        Instance.ambientSource.volume = Instance.ambientVolume * Instance.masterVolume;
    }

    public void SetMusicVolume(float musicVolume)
    {
        Instance.musicVolume = musicVolume;
        Instance.musicSource.volume = Instance.musicVolume * Instance.masterVolume;
    }
    public void SetSFXVolume(float sfxVolume)
    {
        Instance.sfxVolume = sfxVolume;
        Instance.sfxSource.volume = Instance.sfxVolume * Instance.masterVolume;
    }
    public void SetAmbianceVolume(float ambianceVolume)
    {
        Instance.ambientVolume = ambianceVolume;
        Instance.ambientSource.volume = Instance.ambientVolume * Instance.masterVolume;
    }
    #endregion
}

