using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateMachine : MonoBehaviour
{

    private BaseState currentState;

    public NPCIdleState idleState;
    public NPCClickState clickState;
    
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Transform transform = GetComponent<Transform>();

        idleState = new NPCIdleState(agent, transform);
        clickState = new NPCClickState(agent, transform);

        currentState = idleState;
        currentState.EnterState();
    }

    void Update()
    {
        currentState.UpdateState();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TransitionToState(clickState);
        }
    }

    public void TransitionToState(BaseState state)
    {
        currentState.ExitState();
        currentState = state;
        currentState.EnterState();
    }
}
