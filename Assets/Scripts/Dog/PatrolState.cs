using NaughtyAttributes;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : DogState
{
    private enum State { Wander, Idle }
    [ReadOnly][SerializeField] private State currentState = State.Idle;

    [Header("Properties")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float rotateDuration = 1f;

    // Wander
    [SerializeField] private RangeFloat wanderRadius = new(.5f, 2f);
    // Idle
    [SerializeField] private RangeFloat idleDuration = new(1, 4);

    [Header("Wander Lookup")]
    [SerializeField] private int maxAttempts;

    // Idle
    private float timer;
    private float stopIdlingTime;
    private Vector3 nextDestination;
    private Vector3 prevDestination;
    private Vector3 prevPosition;


    public override void EnterState(DogStateMachine stateMachine)
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        float random = Random.Range(0f, 1f);
        if (random < 0.5f)
        {
            SwitchToWander();
        }
        else
        {
            SwitchToIdle();
        }
    }

    public override void UpdateState(DogStateMachine stateMachine)
    {
        if (!fieldOfView.isPlayerVisible || player.GetComponent<PlayerMovement>().IsHiding())
        {
            switch (currentState)
            {
                case State.Wander:
                    WanderLoop();
                    break;
                case State.Idle:
                    IdleLoop();
                    break;
            }
        }
        else
        {
            stateMachine.ChangeState(stateMachine.ChaseState);
        }
    }

    public override void ExitState(DogStateMachine stateMachine)
    {
        animator.SetBool("Patrol", false);
        agent.ResetPath();
    }

    private void AlignOrientation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f) spriteRenderer.flipX = agent.velocity.x >= 0;
    }

    private void SwitchToIdle()
    {
        //print("Switching to idle");
        if (agent == null) return;
        nextDestination = GetValidDestination();
        animator.SetBool("Patrol", false);
        agent.ResetPath();
        timer = 0f;
        stopIdlingTime = Random.Range(idleDuration.min, idleDuration.max);

        currentState = State.Idle;
    }

    private IEnumerator WaitDelay()
    {
        WaitForSeconds wait = new WaitForSeconds(rotateDuration);

        yield return wait;
    }

    private void SwitchToWander()
    {
        print("Switching to wander");
        StartCoroutine(WaitDelay());
        if (agent == null) return;

        if (nextDestination == null)
        {
            nextDestination = GetValidDestination();
        }

        prevPosition = transform.position;
        agent.speed = speed;
        fieldOfView.SetVisionDirection(transform.position, nextDestination);
        agent.SetDestination(nextDestination);
        animator.SetBool("Patrol", true);

        currentState = State.Wander;
    }

    private void WanderLoop()
    {
        AlignOrientation();

        bool isPathStale = agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial;
        bool isPathPending = agent.pathPending;
        bool isDestinationReached = agent.remainingDistance <= agent.stoppingDistance;

        if (isPathStale || isPathPending || isDestinationReached)
        {
            SwitchToIdle();
        }
    }

    private void IdleLoop()
    {
        timer += Time.deltaTime;
        if (timer >= stopIdlingTime)
        {
            SwitchToWander();
        }
        else
        {
            print("Rotating target: " + nextDestination);
            print("Rotating value current: " + Vector3.Lerp(Vector3.forward, nextDestination, timer / stopIdlingTime));
            //print("");
            fieldOfView.SetVisionDirection(transform.position, Vector3.Lerp(prevPosition == null ? Vector3.forward : transform.position + (transform.position -  prevPosition).normalized, nextDestination, timer / stopIdlingTime));
        }
    }

    private Vector3 GetValidDestination()
    {
        // EXPECTED: Get Random Destination inside the wander radius
        for (int i = 0; i < maxAttempts; i++)
        {
            float radius = wanderRadius.RandomValue();
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            Vector3 targetPos = transform.position + randomDirection;

            // Debug.DrawLine(transform.position, targetPos, Color.red, 2f);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                // Debug.DrawLine(transform.position, hit.position, Color.green, 2f);
                // Check if the path is full
                var path = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        fieldOfView.SetVisionDirection(transform.position, hit.position);
                        return hit.position;
                    }
                }
            }
        }

        // FALLBACK 1: Get Random Destination in wherever
        NavMeshHit randomHit;
        if (NavMesh.SamplePosition(Random.insideUnitSphere * 1000f + transform.position, out randomHit, 1000f, NavMesh.AllAreas))
        {
            fieldOfView.SetVisionDirection(transform.position, randomHit.position);
            return randomHit.position;
        }

        // FALLBACK 2: Stay in the same position
        fieldOfView.SetVisionDirection(transform.position, transform.position);
        return transform.position;
    }
}
