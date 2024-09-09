using UnityEngine;
using UnityEngine.AI;

public class CatStrayWanderState : CatBaseState {
    public CatStrayWanderState(CatStateMachine stm) : base(stm) { }

    public override void EnterState()
    {
        stm.Agent.speed = stm.Wander.Speed;
        stm.Agent.SetDestination(GetValidDestination());
    }

    public override void UpdateState()
    {
        bool isPathStale = stm.Agent.pathStatus == NavMeshPathStatus.PathInvalid || stm.Agent.pathStatus == NavMeshPathStatus.PathPartial;
        bool isPathPending = stm.Agent.pathPending;
        bool isDestinationReached = stm.Agent.remainingDistance <= stm.Agent.stoppingDistance;

        if (isPathStale || isPathPending || isDestinationReached) {
            stm.ChangeState(stm.STATE_STRAYWANDER);
        }

        stm.AlignOrientation();
    }

    public override void ExitState()
    {
        stm.Agent.ResetPath();
    }

    private Vector3 GetValidDestination()
    {
        // EXPECTED: Get Random Destination inside the wander radius
        for (int i = 0; i < 30; i++)
        {
            float radius = stm.Wander.Radius.RandomValue();
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            Vector3 targetPos = stm.transform.position + randomDirection;

            // Debug.DrawLine(transform.position, targetPos, Color.red, 2f);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                // Debug.DrawLine(transform.position, hit.position, Color.green, 2f);
                // Check if the path is full
                var path = new NavMeshPath();
                if (NavMesh.CalculatePath(stm.transform.position, hit.position, NavMesh.AllAreas, path)) {
                    if (path.status == NavMeshPathStatus.PathComplete) return hit.position;
                }
            }
        }

        // FALLBACK 1: Get Random Destination in wherever
        NavMeshHit randomHit;
        if (NavMesh.SamplePosition(Random.insideUnitSphere * 1000f + stm.transform.position, out randomHit, 1000f, NavMesh.AllAreas))
        {
            return randomHit.position;
        }

        // FALLBACK 2: Stay in the same position
        return stm.transform.position;
    }
}