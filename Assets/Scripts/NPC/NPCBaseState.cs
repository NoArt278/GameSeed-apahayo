public abstract class NPCBaseState
{
    protected NPCStateMachine STM;
    public bool FacingRight = false;

    public NPCBaseState(NPCStateMachine stm) { STM = stm; }

    public virtual void EnterState() {}
    public virtual void UpdateState() {}
    public virtual void ExitState() {}
}
