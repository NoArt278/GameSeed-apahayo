using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : DogState
{
    private enum State { Wander, Idle }
    [ReadOnly][SerializeField] private State currentState = State.Idle;

    [Header("Properties")]
    [SerializeField] private float speed = 3.5f;
    // Wander
    [SerializeField] private RangeFloat wanderRadius = new(.5f, 2f);
    // Idle
    [SerializeField] private RangeFloat idleDuration = new(1, 4);

    [Header("Wander Lookup")]
    [SerializeField] private int maxAttempts;

    // Idle
    private float timer;
    private float stopIdlingTime;

    public override void EnterState(DogStateMachine stateMachine)
    {
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
        if (!fieldOfView.isPlayerVisible)
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
        agent.ResetPath();
    }

    private void AlignOrientation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f) spriteRenderer.flipX = agent.velocity.x >= 0;
    }

    private void SwitchToIdle()
    {
        if (agent == null) return;
        agent.ResetPath();
        timer = 0f;
        stopIdlingTime = Random.Range(idleDuration.min, idleDuration.max);

        currentState = State.Idle;
    }

    private void SwitchToWander()
    {
        if (agent == null) return;
        agent.speed = speed;
        agent.SetDestination(GetValidDestination());

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
