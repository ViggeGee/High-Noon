using Unity.VisualScripting;
using UnityEngine;

public class SpinChamberState : BaseState
{
    float animationTimer = 5f;
    float timer = 5;
    public override void EnterState(RRStateManager agent)
    {
        agent.animator.SetBool("RR_SpinChamberState", true);
        agent.animator.SetBool("RR_IdleState", false);
        Debug.Log("Entered SpinChamberState");
    }
    public override void UpdateState(RRStateManager agent)
    {

        timer += Time.deltaTime;
        if (timer > animationTimer)
        {
            agent.SwitchState(agent.idleState);
        }
    }
}
