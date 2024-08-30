using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState
{
    protected NavMeshAgent agent;
    protected Transform transform;
    public bool facingRight = false;

    public BaseState(NavMeshAgent agent, Transform transform)
    {
        this.agent = agent;
        this.transform = transform;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
