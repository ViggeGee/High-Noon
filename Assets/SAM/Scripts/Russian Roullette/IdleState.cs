using UnityEngine;

public class IdleState : BaseState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void EnterState(RRStateManager agent)
    {
        agent.animator.SetBool("RR_IdleState", true);
        agent.animator.SetBool("RR_SpinChamberState", false);
    }
    public override void UpdateState(RRStateManager agent)
    {

    }
}
