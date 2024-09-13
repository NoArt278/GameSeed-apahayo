using UnityEngine;
using UnityEngine.AI;

public class NPCWanderState : NPCBaseState
{
    private readonly NavMeshAgent agent;
    private readonly SpriteRenderer spriteRenderer;

    private Vector3 randomPoint;

    public NPCWanderState(NPCStateMachine stm) : base(stm)
    {
        agent = stm.Agent;
        spriteRenderer = stm.SpriteRenderer;
    }

    public override void EnterState()
    {
        for (int i = 0; i < 30; i++)
        {
            if (GetRandomPositionOnNavMesh(out randomPoint)) break;
        }

        agent.SetDestination(randomPoint);
        AlignOrientation();

        spriteRenderer.color = Color.white;

        GameTimer.Instance.AddTime(5f);

        stm.Collider.enabled = false;

        // stm.animator.SetBool("isHypno", true);
    }

    public override void UpdateState()
    {   
        AlignOrientation();
        CheckArrival();
    }

    private void CheckArrival()
    {
        if (HasArrived()) stm.TransitionToState(stm.STATE_IDLE);
    }

    private bool HasArrived()
    {
        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            return true;
        }

        return false;
    }

    bool GetRandomPositionOnNavMesh(out Vector3 result)
    {
        Bounds bounds = stm.Surface.navMeshData.sourceBounds;

        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            var path = new NavMeshPath();
            if (NavMesh.CalculatePath(stm.transform.position, hit.position, NavMesh.AllAreas, path))
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
        if (agent.velocity.sqrMagnitude > 0.1f) spriteRenderer.flipX = agent.velocity.x < 0;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(stm.transform.position, randomPoint);
    }
}
