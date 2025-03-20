using UnityEngine;

public class TriggerBird : MonoBehaviour
{
    public bool triggerBirds = false;
    float timer = 0;

    private void Update()
    {
        if(triggerBirds == true)
        {
            timer += Time.deltaTime;
            if (timer >= 15)
            {                
                triggerBirds = false;
                timer = 0;
            }

        }
    }
}
