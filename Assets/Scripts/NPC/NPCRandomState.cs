using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCRandomState : BaseState
{
    public float range = 10.0f;
    private NavMeshAgent agent;
    private Transform transform;
    private Vector3 randomPoint;
    public float timewait = 0;
    public int waitTimeMin = 2;
    public int waitTimeMax = 5;

    public bool isCountingDown = false;

    public NPCRandomState(MonoBehaviour monoBehaviour) : base(monoBehaviour)
    {
        agent = monoBehaviour.GetComponent<NavMeshAgent>();
        transform = monoBehaviour.transform;
    }

    public override void EnterState()
    {
        agent.updateRotation = false;
        timewait = Random.Range(waitTimeMin, waitTimeMax);
    }

    public override void UpdateState()
    {
        FlipRotation();
        CheckArrival();
        if(!isCountingDown){
            agent.SetDestination(randomPoint);
        }
    }

    private void CheckArrival()
    {
        if (Vector3.Distance(transform.position, randomPoint) < 1.0f)
        {
            RandomPoint(transform.position, range, out randomPoint);
            monoBehaviour.StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown() {
        isCountingDown = true;
        Debug.Log("Waiting for " + timewait + " seconds");
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

    void FlipRotation(){
        Vector3 direction = agent.velocity;

        if(facingRight && direction.x < 0 || !facingRight && direction.x > 0){
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
        }
        
    }
}
