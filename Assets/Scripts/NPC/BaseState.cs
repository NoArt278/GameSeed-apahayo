using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState
{
    protected MonoBehaviour monoBehaviour;
    public bool facingRight = false;

    public BaseState(MonoBehaviour monoBehaviour)
    {
        this.monoBehaviour = monoBehaviour;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
