using UnityEngine;

public class RRStateManager : MonoBehaviour
{

    //-----------FSM STATES-------------
    public BaseState currentState;

    IdleState idleState = new();
    SpinChamberState spinChamberState = new();
    AimRevolverState aimRevolverState = new();
    MissBulletState missBulletState = new();
    HitBulletState hitBulletState = new();
    DeathState deathState = new();

    //-----------COMPONENTS---------------

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentState.EnterState(this);

    }

    // Update is called once per frame
    void Update()
    {

        currentState.UpdateState(this);

    }

    public void SwitchState(BaseState state)
    {

        currentState = state;
        state.EnterState(this);

    }

}
