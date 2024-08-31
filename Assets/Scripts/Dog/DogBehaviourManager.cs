using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBehaviourManager : MonoBehaviour
{
    private PatrolBehaviour patrolBehaviour;
    private ChaseBehaviour chaseBehaviour;
    [SerializeField] private FieldOfView fieldOfView;

    public enum State { Patrol, Chase }
    public State CurrentState { get; private set; } = State.Patrol;
    public Action <State, State> OnStateChanged;
    public Action OnDestroyed;

    private void Awake()
    {
        patrolBehaviour = GetComponent<PatrolBehaviour>();
        chaseBehaviour = GetComponent<ChaseBehaviour>();

        Patrol();
    }

    private void ChangeState(State newState)
    {
        if (CurrentState == newState) return;

        State previousState = CurrentState;
        CurrentState = newState;
        OnStateChanged?.Invoke(previousState, CurrentState);
    }

    public void Patrol()
    {
        patrolBehaviour.enabled = true;
        chaseBehaviour.enabled = false;

        ChangeState(State.Patrol);
    }

    public void Chase()
    {
        patrolBehaviour.enabled = false;
        chaseBehaviour.enabled = true;

        ChangeState(State.Chase);
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}
