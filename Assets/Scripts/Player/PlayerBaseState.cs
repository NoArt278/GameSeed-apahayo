
public abstract class PlayerBaseState {
    protected PlayerStateMachine stm;

    public PlayerBaseState(PlayerStateMachine stm) {
        this.stm = stm;
    }

    public virtual void EnterState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}