using UnityEngine;
using UnityEngine.AI;

public class NPCWayPointState : NPCBaseState
{
    private NavMeshAgent _agent;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _randomWaypoint;

    public NPCWayPointState(NPCStateMachine stm) : base(stm)
    {
        _agent = stm.GetComponent<NavMeshAgent>();
        _spriteRenderer = stm.GetComponentInChildren<SpriteRenderer>();
    }

    public override void EnterState()
    {
        for (int i = 0; i < 30; i++)
        {
            if (GetRandomWaypoint(out _randomWaypoint)) break;
        }

        _agent.SetDestination(_randomWaypoint);
    }

    public override void UpdateState()
    {   
        CheckArrival();
        AlignOrientation();
    }

    private void CheckArrival()
    {
        if (_agent.remainingDistance < 0.3f)
        {
            STM.TransitionToState(STM.STATE_IDLE);
        }
    }

    private bool GetRandomWaypoint(out Vector3 result)
    {
        // TODO: Ganti Ke Waypoint Randomnya

        Bounds bounds = STM.Surface.navMeshData.sourceBounds;

        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero; // Fallback in case no valid position is found
        return false;
    }

    private void AlignOrientation(){
        if (_agent.velocity.sqrMagnitude > 0.1f) _spriteRenderer.flipX = _agent.velocity.x > 0;
    }
}
