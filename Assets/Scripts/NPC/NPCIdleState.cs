using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCIdleState : NPCBaseState 
{
    public float Timewait = 0;
    public RangeFloat WaitTime = new RangeFloat(1, 5);
    private readonly NavMeshAgent _agent;
    private Coroutine _idleCoroutine;

    public NPCIdleState(NPCStateMachine stm) : base(stm) {
        _agent = stm.Agent;
    }

    public override void EnterState()
    {
        _agent.ResetPath();
        Timewait = WaitTime.RandomValue();
        // Debug.Log("Idle for " + timewait + " seconds");
        _idleCoroutine = STM.StartCoroutine(Idle());
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(Timewait);
        STM.TransitionToState(STM.STATE_RANDOMMOVE);
    }

    public override void ExitState()
    {
        if (_idleCoroutine != null) STM.StopCoroutine(_idleCoroutine);
    }

}

