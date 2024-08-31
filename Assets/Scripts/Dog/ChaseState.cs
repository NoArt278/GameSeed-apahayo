using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : DogState
{
    private GameObject Player;
    public bool shouldChase = false;
    private bool lostTimerStarted = false;
    
    private void AlignOrientation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f) spriteRenderer.flipX = agent.velocity.x >= 0;
    }

    public override void EnterState(DogStateMachine stateMachine)
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        agent.destination = Player.transform.position;
        shouldChase = true;
        print("Enter Chase State");
    }

    public override void UpdateState(DogStateMachine stateMachine)
    {
        if (!fieldOfView.isPlayerVisible && !lostTimerStarted)
        {
            lostTimerStarted = true;
            StartCoroutine(TargetLostRoutine());
        }

        if (shouldChase)
        {
            agent.destination = Player.transform.position;
            fieldOfView.SetVisionDirection(transform.position, Player.transform.position);
            AlignOrientation();
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

    private IEnumerator TargetLostRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(2);

        yield return delay;

        shouldChase = fieldOfView.isPlayerVisible;

        lostTimerStarted = false;      
    }


}
