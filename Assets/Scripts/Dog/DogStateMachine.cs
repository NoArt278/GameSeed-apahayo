using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogStateMachine : MonoBehaviour
{
    public PatrolState PatrolState;
    public ChaseState ChaseState;

    public DogState CurrentState;

    private void Awake()
    {
        PatrolState = GetComponent<PatrolState>();
        ChaseState = GetComponent<ChaseState>();

        ChangeState(PatrolState);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        if(CurrentState != null)
        {
            CurrentState.UpdateState(this);
        }
    }

    public void ChangeState(DogState newState)
    {
        if (CurrentState == newState) return;

        if(CurrentState != null)
        {
            CurrentState.ExitState(this);
        }

        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    private void OnBecameVisible() {
        // AudioManager.Instance.Play("Sniff");
    }
}
