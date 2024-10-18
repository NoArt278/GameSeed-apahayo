using UnityEngine;
using UnityEngine.AI;

public class NPCWanderState : NPCBaseState
{
    private readonly NavMeshAgent _agent;
    private readonly SpriteRenderer _spriteRenderer;

    private Vector3 _randomPoint;

    public NPCWanderState(NPCStateMachine stm) : base(stm)
    {
        _agent = stm.Agent;
        _spriteRenderer = stm.SpriteRenderer;
    }

    public override void EnterState()
    {
        for (int i = 0; i < 30; i++)
        {
            if (GetRandomPositionOnNavMesh(out _randomPoint)) break;
        }

        _agent.SetDestination(_randomPoint);
        AlignOrientation();

        _spriteRenderer.color = Color.white;

        GameTimer.Instance.AddTime(STM.HypnotizeStats.hypnotizeHealth / 3.7f);

        STM.Collider.enabled = false;
    }

    public override void UpdateState()
    {   
        AlignOrientation();
        CheckArrival();
    }

    private void CheckArrival()
    {
        // if (HasArrived()) EnterState();
        if (HasArrived()) STM.TransitionToState(STM.STATE_IDLE);
    }

    private bool HasArrived()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance + 0.1f)
        {
            return true;
        }

        return false;
    }

    bool GetRandomPositionOnNavMesh(out Vector3 result)
    {
        Bounds bounds = STM.Surface.navMeshData.sourceBounds;

        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            var path = new NavMeshPath();
            if (NavMesh.CalculatePath(STM.transform.position, hit.position, NavMesh.AllAreas, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    result = hit.position;
                    return true;
                }
            }
        }

        result = Vector3.zero; // Fallback in case no valid position is found
        return false;
    }

    void AlignOrientation(){
        if (_agent.velocity.sqrMagnitude > 0.1f) _spriteRenderer.flipX = _agent.velocity.x < 0;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(STM.transform.position, _randomPoint);
    }


}
