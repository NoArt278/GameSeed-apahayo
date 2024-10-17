public abstract class CatBaseState {
    protected CatStateMachine stm;

    public CatBaseState(CatStateMachine stm) {
        this.stm = stm;
    }

    public virtual void EnterState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}