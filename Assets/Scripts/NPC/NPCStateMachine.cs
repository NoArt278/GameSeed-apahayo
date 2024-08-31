using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateMachine : MonoBehaviour
{

    private BaseState currentState;

    public NPCIdleState idleState;
    public NPCClickState clickState;
    public NPCRandomState randomState;
    
    private int stateIndex = 0;

    private BaseState[] states;
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Transform transform = GetComponent<Transform>();

        idleState = new NPCIdleState(agent, transform);
        clickState = new NPCClickState(agent, transform);
        randomState = new NPCRandomState(agent, transform);

        states = new BaseState[] {idleState, clickState, randomState};

        currentState = states[stateIndex];
        currentState.EnterState();
    }

    void Update()
    {
        currentState.UpdateState();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateIndex = (stateIndex + 1) % states.Length;
            TransitionToState(states[stateIndex]);
            Debug.Log("Switching to state: " + currentState.GetType().Name);
        }
    }

    public void TransitionToState(BaseState state)
    {
        currentState.ExitState();
        currentState = state;
        currentState.EnterState();
    }
}
