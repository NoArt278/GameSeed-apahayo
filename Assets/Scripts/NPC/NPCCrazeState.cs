using UnityEngine;
using UnityEngine.AI;

public class NPCCrazeState : NPCBaseState
{
    private readonly NavMeshAgent agent;
    private readonly SpriteRenderer spriteRenderer;
    private float wanderRadius = 10f;
    public int crazeSpeed = 20;

    public NPCCrazeState(NPCStateMachine stm) : base(stm)
    {
        agent = stm.Agent;
        spriteRenderer = stm.SpriteRenderer;
    }


    public override void EnterState()
    {
        agent.speed = crazeSpeed;
        agent.acceleration = crazeSpeed * 2;
        agent.SetDestination(GetValidDestination());

        GameTimer.Instance.AddTime(5f);

        stm.SpriteRenderer.color = Color.red;
    }


    public override void UpdateState() {
        if (agent.remainingDistance < 0.3f) {
            agent.SetDestination(GetValidDestination());
        }
        
        AlignOrientation();

        if (IsOutOfCamera()){
            Object.Destroy(stm.gameObject);
        } 
    }

    void AlignOrientation(){
        if (agent.velocity.sqrMagnitude > 0.1f) spriteRenderer.flipX = agent.velocity.x < 0;
    }


    private Vector3 GetValidDestination()
    {
        // EXPECTED: Get Random Destination inside the wander radius
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            Vector3 targetPos = stm.transform.position + randomDirection;

            // Debug.DrawLine(transform.position, targetPos, Color.red, 2f);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
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

    public bool IsOutOfCamera() {
        Vector2 clipSpace = Camera.main.WorldToViewportPoint(stm.transform.position + stm.SpriteRenderer.bounds.extents.y * Vector3.up);
        if (clipSpace.x < 0 || clipSpace.x > 1 || clipSpace.y < 0 || clipSpace.y > 1)
        {
            return true;
        }
        return false;
    }
}
