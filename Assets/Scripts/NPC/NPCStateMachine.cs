using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateMachine : MonoBehaviour
{
    // states
    private BaseState currentState;
    private NPCIdleState idleState;
    private NPCClickState clickState;
    private NPCRandomState randomState;
    private NPCWayPointState wayPointState;
    private NPCHypnotizedState hypnotizedState;
    private int stateIndex = 0;
    private BaseState[] states;

    // script component
    private HypnotizeManager hypnotizeManager;

    void Start()
    {
        // initialize states
        idleState = new NPCIdleState(this);
        clickState = new NPCClickState(this);
        randomState = new NPCRandomState(this);
        wayPointState = new NPCWayPointState(this);
        hypnotizedState = new NPCHypnotizedState(this);

        // reference another script    
        hypnotizeManager = GetComponent<HypnotizeManager>();

        // initialize state array
        states = new BaseState[] {idleState, clickState, randomState, wayPointState, hypnotizedState};

        // set initial state
        currentState = states[stateIndex];
        currentState.EnterState();
    }

    void Update()
    {
        currentState.UpdateState();

        checkHypnotize();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateIndex = (stateIndex + 1) % states.Length;
            TransitionToState(states[stateIndex]);
            Debug.Log("Switching to state: " + currentState.GetType().Name);
        }
    }

    public void TransitionToState(BaseState state)
    {
        Debug.Log("Switching to state: " + state.GetType().Name);
        currentState.ExitState();
        currentState = state;
        currentState.EnterState();
    }

    private void checkHypnotize()
    {
        if (hypnotizeManager.isHypnotized)
        {
            TransitionToState(hypnotizedState); // TODO: hypnotizing state
        }
        else
        {
            TransitionToState(wayPointState);
        }

    }
}
