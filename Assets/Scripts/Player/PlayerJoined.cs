using UnityEngine;

public class PlayerJoined : MonoBehaviour
{
    private bool isPlayerHost = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
   
    public void SetIsPlayerHost(bool trueOrFalse)
    {
        isPlayerHost = trueOrFalse;
    }
    public bool GetIsPlayerHost()
    {
        return isPlayerHost;
    }
}
