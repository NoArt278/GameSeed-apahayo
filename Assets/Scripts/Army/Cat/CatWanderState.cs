using UnityEngine;
using UnityEngine.AI;

public class CatStrayWanderState : CatBaseState {
    public CatStrayWanderState(CatStateMachine stm) : base(stm) { }

    public override void EnterState()
    {
        STM.Agent.speed = STM.Wander.Speed;
        STM.Agent.SetDestination(GetValidDestination());
    }

    public override void UpdateState()
    {
        bool isPathStale = STM.Agent.pathStatus == NavMeshPathStatus.PathInvalid || STM.Agent.pathStatus == NavMeshPathStatus.PathPartial;
        bool isPathPending = STM.Agent.pathPending;
        bool isDestinationReached = STM.Agent.remainingDistance <= STM.Agent.stoppingDistance;

        if (isPathStale || isPathPending || isDestinationReached) {
            STM.ChangeState(STM.STATE_STRAYIDLE);
        }

        STM.AlignOrientation();
    }

    public override void ExitState()
    {
        STM.Agent.ResetPath();
    }

    private Vector3 GetValidDestination()
    {
        // EXPECTED: Get Random Destination inside the wander radius
        for (int i = 0; i < 30; i++)
        {
            float radius = STM.Wander.Radius.RandomValue();
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            Vector3 targetPos = STM.transform.position + randomDirection;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
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