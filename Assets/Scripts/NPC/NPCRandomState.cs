using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NPCRandomState : BaseState
{
    public float range = 10.0f;
    private NavMeshAgent agent;
    private Transform transform;
    private Vector3 randomPoint;
    public float timewait = 0;
    public int waitTimeMin = 0;
    public int waitTimeMax = 5;

    public bool isCountingDown = false;
    private SpriteRenderer spriteRenderer;
    private NavMeshSurface navMeshSurface;

    

    public NPCRandomState(MonoBehaviour monoBehaviour) : base(monoBehaviour)
    {
        agent = monoBehaviour.GetComponent<NavMeshAgent>();
        transform = monoBehaviour.transform;
        spriteRenderer = monoBehaviour.GetComponentInChildren<SpriteRenderer>();
        navMeshSurface = GameObject.FindGameObjectWithTag("WalkableSurface").GetComponent<NavMeshSurface>();

        GetRandomPositionOnNavMesh(out randomPoint);
    }

    public override void EnterState()
    {
        agent.updateRotation = false;
        timewait = Random.Range(waitTimeMin, waitTimeMax);
    }

    public override void UpdateState()
    {
        FlipSprite();
        CheckArrival();
        if(!isCountingDown){
            agent.SetDestination(randomPoint);
        }

        if(agent.pathStatus == NavMeshPathStatus.PathInvalid){
            GetRandomPositionOnNavMesh(out randomPoint);
        }
    }

    private void CheckArrival()
    {
        if (Vector3.Distance(transform.position, randomPoint) < 1.0f && GetRandomPositionOnNavMesh(out randomPoint))
        {
            monoBehaviour.StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown() {
        isCountingDown = true;
        // Debug.Log("Waiting for " + timewait + " seconds");
        yield return new WaitForSeconds(timewait);
        timewait = Random.Range(waitTimeMin, waitTimeMax);
        isCountingDown = false;
    }
    
    public override void ExitState()
    {
        
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        { 
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    bool GetRandomPositionOnNavMesh(out Vector3 result)
    {
        Bounds bounds = navMeshSurface.navMeshData.sourceBounds;

        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, 5f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero; // Fallback in case no valid position is found
        return false;
    }

    void FlipRotation(){
        Vector3 direction = agent.velocity;

        if(facingRight && direction.x < 0 || !facingRight && direction.x > 0){
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
        }
        
    }

    void FlipSprite(){
        if (agent.velocity.sqrMagnitude > 0.1f) spriteRenderer.flipX = agent.velocity.x > 0;
    }
}
