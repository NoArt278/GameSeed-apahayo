using UnityEngine;
using UnityEngine.AI;

public class NPCRandomMoveState : NPCBaseState
{
    private readonly NavMeshAgent agent;
    private readonly SpriteRenderer spriteRenderer;

    private Vector3 randomPoint;

    public NPCRandomMoveState(NPCStateMachine stm) : base(stm)
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

        // Debug.Log("Random Move to " + randomPoint);

        agent.SetDestination(randomPoint);
        AlignOrientation();

        spriteRenderer.color = Color.white;
    }

    public override void UpdateState()
    {   
        AlignOrientation();
        CheckArrival();
        // stm.IsNPCWalking();
    }

    private void CheckArrival()
    {
        if (HasArrived()) stm.TransitionToState(stm.STATE_IDLE);
    }

    private bool HasArrived()
    {
        return IsWithinStoppingDistance(agent.transform.position, agent.destination, agent.stoppingDistance);
    }

    private bool IsWithinStoppingDistance(Vector3 currentPosition, Vector3 destinationPosition, float stoppingDistance)
    {
        float distance = Vector3.Distance(currentPosition, destinationPosition);
        return distance <= stoppingDistance + 0.1f;
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
