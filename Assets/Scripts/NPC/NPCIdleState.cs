using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCIdleState : BaseState 
{

    public NPCIdleState(NavMeshAgent agent, Transform transform) : base(agent, transform)
    {
    }

    public override void EnterState()
    {
        agent.updateRotation = false;
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
        
    }

}

