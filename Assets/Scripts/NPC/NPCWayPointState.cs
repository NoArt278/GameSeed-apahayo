using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCWayPointState : BaseState
{
    GameObject[] wayPoints;
    int wayPointIndex = 0;
    bool isCountingDown = false;
    public int waitTimeMin = 2;
    public int waitTimeMax = 5;

    public float timewait = 0;
    private NavMeshAgent agent;
    private Transform transform;
    private SpriteRenderer spriteRenderer;

    public NPCWayPointState(MonoBehaviour monoBehaviour) : base(monoBehaviour)
    {
        agent = monoBehaviour.GetComponent<NavMeshAgent>();
        transform = monoBehaviour.transform;
        spriteRenderer = monoBehaviour.GetComponentInChildren<SpriteRenderer>();

        timewait = Random.Range(waitTimeMin, waitTimeMax);
    }
    public override void EnterState()
    {
        agent.updateRotation = false;
        wayPoints = GameObject.FindGameObjectsWithTag("WayPoint");
    }

    public override void UpdateState()
    {
        FlipRotation();
        CheckArrival();
        if(!isCountingDown){
            MoveToWayPoint();
        }
    }
    public override void ExitState()
    {
    }
    private void CheckArrival() {
        if (Vector3.Distance(transform.position, wayPoints[wayPointIndex].transform.position) < 1.0f) {
            wayPointIndex = (wayPointIndex + 1) % wayPoints.Length;
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
    private void MoveToWayPoint() {
        agent.SetDestination(wayPoints[wayPointIndex].transform.position);
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
