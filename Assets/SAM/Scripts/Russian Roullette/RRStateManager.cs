using UnityEngine;

public class RRStateManager : MonoBehaviour
{

    //-----------FSM STATES-------------
    public BaseState currentState;

    public StartState startState = new ();
    public IdleState idleState = new();
    public SpinChamberState spinChamberState = new();
    public AimRevolverState aimRevolverState = new();
    public MissBulletState missBulletState = new();
    public HitBulletState hitBulletState = new();
    public DeathState deathState = new();

    //-----------COMPONENTS---------------

    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentState = startState;
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
