using UnityEngine;

public abstract class BaseState
{
    public abstract void EnterState(RRStateManager agent);

    public abstract void UpdateState(RRStateManager agent);
}
