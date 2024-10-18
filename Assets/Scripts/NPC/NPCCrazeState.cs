using UnityEngine;
using UnityEngine.AI;

public class NPCCrazeState : NPCBaseState
{
    private readonly NavMeshAgent _agent;
    private readonly SpriteRenderer _spriteRenderer;
    private readonly float _wanderRadius = 10f;
    public int CrazeSpeed = 20;

    public NPCCrazeState(NPCStateMachine stm) : base(stm)
    {
        _agent = stm.Agent;
        _spriteRenderer = stm.SpriteRenderer;
    }


    public override void EnterState()
    {
        _agent.speed = CrazeSpeed;
        _agent.acceleration = CrazeSpeed * 2;
        _agent.SetDestination(GetValidDestination());

        // GameTimer.Instance.AddTime(stm.HypnotizeStats.hypnotizeHealth / 2);
    }


    public override void UpdateState() {
        if (_agent.remainingDistance < 0.3f) {
            _agent.SetDestination(GetValidDestination());
        }
        
        AlignOrientation();
    }

    void AlignOrientation(){
        if (_agent.velocity.sqrMagnitude > 0.1f) _spriteRenderer.flipX = _agent.velocity.x < 0;
    }


    private Vector3 GetValidDestination()
    {
        // EXPECTED: Get Random Destination inside the wander radius
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * _wanderRadius;
            Vector3 targetPos = STM.transform.position + randomDirection;

            // Debug.DrawLine(transform.position, targetPos, Color.red, 2f);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, _wanderRadius, NavMesh.AllAreas))
            {
                // Debug.DrawLine(transform.position, hit.position, Color.green, 2f);
                // Check if the path is full
                var path = new NavMeshPath();
                if (NavMesh.CalculatePath(STM.transform.position, hit.position, NavMesh.AllAreas, path)) {
                    if (path.status == NavMeshPathStatus.PathComplete) return hit.position;
                }
            }
        }

        // FALLBACK 1: Get Random Destination in wherever
        NavMeshHit randomHit;
        if (NavMesh.SamplePosition(Random.insideUnitSphere * 1000f + STM.transform.position, out randomHit, 1000f, NavMesh.AllAreas))
        {
            return randomHit.position;
        }

        // FALLBACK 2: Stay in the same position
        return STM.transform.position;
    }
}
