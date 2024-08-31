using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : DogState
{
    private GameObject Player;
    public bool shouldChase = false;

    public override void EnterState(DogStateMachine stateMachine)
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        agent.destination = Player.transform.position;
        shouldChase = true;
        print("Enter Chase State");
    }

    public override void UpdateState(DogStateMachine stateMachine)
    {
        if (shouldChase)
        {
            agent.destination = Player.transform.position;
            fieldOfView.SetVisionDirection(transform.position, Player.transform.position);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.PatrolState);
        }
    }

    public override void ExitState(DogStateMachine stateMachine)
    {
        agent.ResetPath();
    }
}
