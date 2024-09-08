using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class DogState : MonoBehaviour
{
    protected NavMeshAgent agent;
    [SerializeField] protected FieldOfView fieldOfView;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;

    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
    }

    public abstract void EnterState(DogStateMachine stateMachine);
    public abstract void UpdateState(DogStateMachine stateMachine);
    public abstract void ExitState(DogStateMachine stateMachine);
    

}
