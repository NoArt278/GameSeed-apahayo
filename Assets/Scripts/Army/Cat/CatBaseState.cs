public abstract class CatBaseState {
    protected CatStateMachine STM;

    public CatBaseState(CatStateMachine stm) {
        this.STM = stm;
    }

    public virtual void EnterState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}