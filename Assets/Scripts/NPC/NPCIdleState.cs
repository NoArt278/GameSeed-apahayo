using System.Collections;
using UnityEngine;

public class NPCIdleState : NPCBaseState 
{
    public float timewait = 0;
    public RangeFloat waitTime = new RangeFloat(1, 5);

    public NPCIdleState(NPCStateMachine stm) : base(stm) {}

    public override void EnterState()
    {
        timewait = waitTime.RandomValue();
        stm.StartCoroutine(Idle());
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(timewait);
        stm.TransitionToState(stm.STATE_RANDOMMOVE);
    }
}

