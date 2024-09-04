using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCHypnotizedState : BaseState
{
    private NavMeshAgent agent;
    private HypnotizeManager hypnotizeManager;

    public NPCHypnotizedState(MonoBehaviour monoBehaviour) : base(monoBehaviour)
    {
        agent = monoBehaviour.GetComponent<NavMeshAgent>();
        hypnotizeManager = monoBehaviour.GetComponent<HypnotizeManager>();
    }

    public override void EnterState()
    {
        agent.isStopped = true;
        agent.updateRotation = false;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        agent.isStopped = false;
    }
}
