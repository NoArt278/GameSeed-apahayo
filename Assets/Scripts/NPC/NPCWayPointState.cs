using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCWayPointState : BaseState
{
    GameObject[] wayPoints;
    public NPCWayPointState(NavMeshAgent agent, Transform transform) : base(agent, transform)
    {
    }
    public override void EnterState()
    {
        agent.updateRotation = false;
        wayPoints = GameObject.FindGameObjectsWithTag("WayPoint");
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
        
    }
}
