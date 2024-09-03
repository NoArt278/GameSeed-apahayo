using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCClickState : BaseState 
{
    private NavMeshAgent agent;
    private Transform transform;
    private SpriteRenderer spriteRenderer;
    
    public NPCClickState(MonoBehaviour monoBehaviour) : base(monoBehaviour)
    {
        agent = monoBehaviour.GetComponent<NavMeshAgent>();
        transform = monoBehaviour.transform;
        spriteRenderer = monoBehaviour.GetComponentInChildren<SpriteRenderer>();
    }

    public override void EnterState()
    {
        agent.updateRotation = false;
    }

    public override void UpdateState()
    {
        FlipSprite();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
        {
            agent.SetDestination(hit.point);
        }
    }

    public override void ExitState()
    {
        
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

