using UnityEngine;

public abstract class NPCBaseState
{
    protected NPCStateMachine stm;
    public bool facingRight = false;

    public NPCBaseState(NPCStateMachine stm)
    {
        this.stm = stm;
    }

    public virtual void EnterState() {}
    public virtual void UpdateState() {}
    public virtual void ExitState() {}
}
