using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : DogState
{
    private enum State { Wander, Idle }
    [ReadOnly, SerializeField] private State currentState = State.Idle;

    [Header("Properties")]
    [SerializeField] private float _speed = 4.1f;
    [SerializeField] private float _rotateDuration = 2f;
    [SerializeField] private float _biasTowardsPlayer = 0.8f;
    [SerializeField] private float _minDistanceAfterHide = 3;

    [SerializeField] private RangeFloat _wanderRadius = new(2f, 12f);
    [SerializeField] private RangeFloat _idleDuration = new(2, 4);


    [Header("Wander Lookup")]
    [SerializeField] private int _maxAttempts = 30;

    // Idle
    private float _timer;
    private float _stopIdlingTime;
    private Vector3 _nextDestination;
    private Vector3 _prevPosition;


    public override void EnterState(DogStateMachine stateMachine)
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
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
        if (!FieldOfView.IsPlayerVisible || Player.GetComponent<PlayerMovement>().IsHiding())
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
        Animator.SetBool("Patrol", false);
        Agent.ResetPath();
    }

    private void AlignOrientation()
    {
        if (Agent.velocity.sqrMagnitude > 0.1f) SpriteRenderer.flipX = Agent.velocity.x >= 0;
    }

    private void SwitchToIdle()
    {
        //print("Switching to idle");
        if (Agent == null) return;
        _nextDestination = GetValidDestination();
        Animator.SetBool("Patrol", false);
        Agent.ResetPath();
        _timer = 0f;
        _stopIdlingTime = Random.Range(_idleDuration.Min, _idleDuration.Max);

        currentState = State.Idle;
    }

    private IEnumerator WaitDelay()
    {
        WaitForSeconds wait = new WaitForSeconds(_rotateDuration);

        yield return wait;
    }

    private void SwitchToWander()
    {
        //print("Switching to wander");
        StartCoroutine(WaitDelay());
        if (Agent == null) return;

        if (_nextDestination == null)
        {
            _nextDestination = GetValidDestination();
        }

        _prevPosition = transform.position;
        Agent.speed = _speed;
        FieldOfView.SetVisionDirection(transform.position, _nextDestination);
        Agent.SetDestination(_nextDestination);
        Animator.SetBool("Patrol", true);

        currentState = State.Wander;
    }

    private void WanderLoop()
    {
        AlignOrientation();

        bool isPathStale = Agent.pathStatus != NavMeshPathStatus.PathComplete;
        bool isPathPending = Agent.pathPending;
        bool isDestinationReached = Agent.remainingDistance <= Agent.stoppingDistance;

        if (isPathStale || isPathPending || isDestinationReached)
        {
            SwitchToIdle();
        }
    }

    private void IdleLoop()
    {
        _timer += Time.deltaTime;
        if (_timer >= _stopIdlingTime)
        {
            SwitchToWander();
        }
        else
        {
            FieldOfView.SetVisionDirection(transform.position, Vector3.Lerp(_prevPosition == null ? Vector3.forward : transform.position + (transform.position -  _prevPosition).normalized, _nextDestination, _timer / _stopIdlingTime));
        }
    }

    private Vector3 GetValidDestination()
    {
        // EXPECTED: Get Random Destination inside the wander radius
        for (int i = 0; i < _maxAttempts; i++)
        {
            float radius = _wanderRadius.RandomValue();
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            Vector3 biasedDirection, targetPos;
            if (Player.GetComponent<PlayerMovement>().IsHiding())
            {
                Vector3 directionAwayFromPlayer = (transform.position - Player.transform.position).normalized;
                biasedDirection = Vector3.Lerp(randomDirection, directionAwayFromPlayer * radius, _biasTowardsPlayer);
                targetPos = transform.position + biasedDirection;

                float targetDistanceToPlayer = Vector3.Distance(targetPos, Player.transform.position);
                if (targetDistanceToPlayer < _minDistanceAfterHide)
                {
                    targetPos = transform.position + directionAwayFromPlayer * radius;
                }
            }
            else
            {
                Vector3 directionToPlayer = (Player.transform.position - transform.position).normalized;
                biasedDirection = Vector3.Lerp(randomDirection, directionToPlayer * radius, _biasTowardsPlayer);
                targetPos = transform.position + biasedDirection;
            }

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                // Check if the path is full
                var path = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        FieldOfView.SetVisionDirection(transform.position, hit.position);

                        
                        return hit.position;
                    }
                }
            }
        }

        // FALLBACK 1: Get Random Destination in wherever
        NavMeshHit randomHit;
        if (NavMesh.SamplePosition(Random.insideUnitSphere * 1000f + transform.position, out randomHit, 1000f, NavMesh.AllAreas))
        {
            FieldOfView.SetVisionDirection(transform.position, randomHit.position);
            return randomHit.position;
        }

        // FALLBACK 2: Stay in the same position
        FieldOfView.SetVisionDirection(transform.position, transform.position);
        return transform.position;
    }
}
