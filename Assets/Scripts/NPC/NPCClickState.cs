using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCClickState : BaseState 
{
    
    public NPCClickState(NavMeshAgent agent, Transform transform) : base(agent, transform)
    {
    }

    public override void EnterState()
    {
        agent.updateRotation = false;
    }

    public override void UpdateState()
    {
        FlipRotation();
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
}

