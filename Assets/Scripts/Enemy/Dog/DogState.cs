using UnityEngine;
using UnityEngine.AI;

public abstract class DogState : MonoBehaviour
{
    protected NavMeshAgent Agent;
    protected GameObject Player;
    [SerializeField] protected FieldOfView FieldOfView;
    [SerializeField] protected SpriteRenderer SpriteRenderer;
    [SerializeField] protected Animator Animator;

    protected void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public abstract void EnterState(DogStateMachine stateMachine);
    public abstract void UpdateState(DogStateMachine stateMachine);
    public abstract void ExitState(DogStateMachine stateMachine);
    

}
