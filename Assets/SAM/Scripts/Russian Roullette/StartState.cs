using UnityEngine;

public class StartState : BaseState
{
    float animationTimer = 5;
    float timer;


    public override void EnterState(RRStateManager agent)
    {
        Debug.Log("Entered StartState");
        timer = 0;
    }

    public override void UpdateState(RRStateManager agent)
    {
        
        timer += Time.deltaTime;

        if (timer > animationTimer)
        {
            agent.SwitchState(agent.spinChamberState);
        }
    }
}
