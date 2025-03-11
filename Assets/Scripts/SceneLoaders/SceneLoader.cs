using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    StartGameScene,
    MainMenu,
    JoinGameScene,

    WesternTown,
    TrainMap,
    //TrainHorseMap,
    Bison,
    TrainHorseMap2
}
public class SceneLoader : NetworkBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("if changeing this. Apply this on the prefab aswell. \n The networkmanager needs the correct prefab on both players")]
    public Scenes gotoThisScene;
    [SerializeField] bool isRandomScene;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    #region Offline
    public void LoadScene(Scenes sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad.ToString());
    }

    #endregion

    #region Online

    public void LoadMainMenuAfterGameIsOver()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(Scenes.MainMenu.ToString(), LoadSceneMode.Single);
        }
    }
    public void LoadRandomSceneForAllPlayers()
    {
       
        if (IsServer)
        {
            if (isRandomScene )
            {
                Scenes randomScene = GetRandomScene();
                LoadSceneForAllPlayersServerRpc(randomScene);
           
            }
            else
            {
                LoadSceneForAllPlayersServerRpc(gotoThisScene);
            }
           
        }
        
        else
        {
            Debug.LogWarning("Only the server can initiate a scene change!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadSceneForAllPlayersServerRpc(Scenes sceneToLoad)
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad.ToString(), LoadSceneMode.Single);
        }
    }

    private Scenes GetRandomScene()
    {
        Array sceneValues = Enum.GetValues(typeof(Scenes));
        return (Scenes)sceneValues.GetValue(UnityEngine.Random.Range(3, sceneValues.Length));
    }
    #endregion
}
