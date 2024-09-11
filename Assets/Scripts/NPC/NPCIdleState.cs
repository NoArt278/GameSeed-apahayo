using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCIdleState : NPCBaseState 
{
    public float timewait = 0;
    public RangeFloat waitTime = new RangeFloat(1, 3);
    private readonly NavMeshAgent agent;
    private Coroutine idleCoroutine;

    public NPCIdleState(NPCStateMachine stm) : base(stm) {
        agent = stm.Agent;
    }

    public override void EnterState()
    {
        agent.ResetPath();
        timewait = waitTime.RandomValue();
        idleCoroutine = stm.StartCoroutine(Idle());
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(timewait);
        stm.TransitionToState(stm.STATE_RANDOMMOVE);
    }

    public override void ExitState()
    {
        if (idleCoroutine != null) stm.StopCoroutine(idleCoroutine);
    }

}

