using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class StrayCatBehaviour : MonoBehaviour {
    private enum State { Wander, Idle }
    [ReadOnly] [SerializeField] private State currentState = State.Idle;
    [SerializeField] private SpriteRenderer catRenderer;

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

    // Wander

    private NavMeshAgent agent;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable() {
        float random = Random.Range(0f, 1f);
        if (random < 0.5f) {
            SwitchToWander();
        } else {
            SwitchToIdle();
        }
    }

    private void Update() {
        switch (currentState) {
            case State.Wander:
                WanderLoop();
                break;
            case State.Idle:
                IdleLoop();
                break;
        }
    }

    private void AlignOrientation() {
        catRenderer.flipX = agent.velocity.x < 0;
    }

    private void SwitchToIdle() {
        agent.ResetPath();
        timer = 0f;
        stopIdlingTime = Random.Range(idleDuration.min, idleDuration.max);

        currentState = State.Idle;
    }

    private void SwitchToWander() {
        agent.speed = speed;
        agent.SetDestination(GetValidDestination());

        currentState = State.Wander;
    }

    private void WanderLoop() {
        AlignOrientation();

        bool isPathStale = agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial;
        bool isPathPending = agent.pathPending;
        bool isDestinationReached = agent.remainingDistance <= agent.stoppingDistance;

        if (isPathStale || isPathPending || isDestinationReached) {
            SwitchToIdle();
        }
    }

    private void IdleLoop() {
        timer += Time.deltaTime;
        if (timer >= stopIdlingTime) {
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
                if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path)) {
                    if (path.status == NavMeshPathStatus.PathComplete) return hit.position;
                }
            }
        }

        // FALLBACK 1: Get Random Destination in wherever
        NavMeshHit randomHit;
        if (NavMesh.SamplePosition(Random.insideUnitSphere * 1000f + transform.position, out randomHit, 1000f, NavMesh.AllAreas))
        {
            return randomHit.position;
        }

        // FALLBACK 2: Stay in the same position
        return transform.position;
    }
}