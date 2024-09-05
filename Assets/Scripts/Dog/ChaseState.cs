using System.Collections;
using UnityEngine;

public class ChaseState : DogState
{
    private GameObject Player;
    [SerializeField] private float chaseRange;
    [SerializeField] private float chaseAngle;
    public bool shouldChase = false;
    private bool lostTimerStarted = false;

    private float prevAngle;
    [SerializeField] private LayerMask obstacleMask;

    private void AlignOrientation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f) spriteRenderer.flipX = agent.velocity.x >= 0;
    }

    public override void EnterState(DogStateMachine stateMachine)
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        agent.destination = Player.transform.position;
        shouldChase = true;
        prevAngle = fieldOfView.Angle;
        fieldOfView.Angle = 360;
        fieldOfView.isChasing = true;
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
        fieldOfView.Angle = prevAngle;
        fieldOfView.isChasing = false;
    }

    private IEnumerator TargetLostRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(2);

        yield return delay;

        shouldChase = fieldOfView.isPlayerVisible;

        lostTimerStarted = false;      
    }


}
