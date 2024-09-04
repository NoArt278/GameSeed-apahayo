using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NPCCrazeState : BaseState
{
    private NavMeshAgent agent;
    private NavMeshSurface navMeshSurface;
    private Vector3 cameraCenter;
    private Camera mainCamera;
    private Vector3 destination;
    private List<Vector3> candidateDestinations;
    public int crazeSpeed = 20;

    public NPCCrazeState(MonoBehaviour monoBehaviour) : base(monoBehaviour)
    {
        agent = monoBehaviour.GetComponent<NavMeshAgent>();
        navMeshSurface = GameObject.FindGameObjectWithTag("WalkableSurface").GetComponent<NavMeshSurface>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        cameraCenter = new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane);

        cameraCenter = mainCamera.ViewportToWorldPoint(cameraCenter);

        candidateDestinations = new List<Vector3> {GetLeftTopCorner(), GetLeftBottomCorner(), GetRightTopCorner(), GetRightBottomCorner()};

        destination = candidateDestinations[0];
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cameraCenter, 0.5f);
    }

    public override void EnterState()
    {
        agent.updateRotation = false;
        GetFurthestPoint();
        agent.SetDestination(destination);
        agent.speed = crazeSpeed;
        agent.acceleration = crazeSpeed * 2;
        Debug.Log(destination);
    }

    public override void UpdateState()
    {
        if (IsObjectOutsideViewport(monoBehaviour.transform.position)){
            UnityEngine.Object.Destroy(monoBehaviour.gameObject);
            HypnotizeManager hypnotizeManager = monoBehaviour.GetComponent<HypnotizeManager>();
            GameObject hypnoBar = hypnotizeManager.hypnoBar.gameObject;

            UnityEngine.Object.Destroy(hypnoBar);
        }
    }

    public override void ExitState()
    {
        
    }

    public bool IsObjectOutsideViewport(Vector3 objectPosition)
    {
        // Convert the object's position to viewport coordinates
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(objectPosition);

        // Check if the object is outside the viewport
        if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
        {
            return true;
        }

        return false;
    }
    

    public void GetFurthestPoint(){
        
        for (int i = 0; i < candidateDestinations.Count; i++){
            if (Vector3.Distance(monoBehaviour.transform.position, candidateDestinations[i]) > Vector3.Distance(monoBehaviour.transform.position, destination)){
                if(IsObjectOutsideViewport(candidateDestinations[i])){
                    destination = candidateDestinations[i];
                }
            }
        }
    }

    public Vector3 GetLeftTopCorner(){
        Bounds bounds = navMeshSurface.navMeshData.sourceBounds;
        return new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);

    }

    public Vector3 GetLeftBottomCorner(){
        Bounds bounds = navMeshSurface.navMeshData.sourceBounds;
        return new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
    }

    public Vector3 GetRightTopCorner(){
        Bounds bounds = navMeshSurface.navMeshData.sourceBounds;
        return new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
    }

    public Vector3 GetRightBottomCorner(){
        Bounds bounds = navMeshSurface.navMeshData.sourceBounds;
        return new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);    
    }

}
