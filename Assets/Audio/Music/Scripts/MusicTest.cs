using UnityEngine;
using UnityEngine.InputSystem;

public class MusicTest : MonoBehaviour
{
    int currentIntensity = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //if a key is pressed
        if (Input.GetKey(KeyCode.Z))
        {
            //play the music
            SimpleAudioManager.Manager.instance.PlaySong(0);

        }
        if(Input.GetKey(KeyCode.X))
        {
            //stop the music
            currentIntensity += 1;
            if(currentIntensity >= 2)
            {
                currentIntensity = 0;
            }
            SimpleAudioManager.Manager.instance.SetIntensity(currentIntensity);
        }
    }
}
