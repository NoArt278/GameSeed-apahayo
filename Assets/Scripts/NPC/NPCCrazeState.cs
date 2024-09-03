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
    public int crazeSpeed = 8;

    public NPCCrazeState(MonoBehaviour monoBehaviour) : base(monoBehaviour)
    {
        agent = monoBehaviour.GetComponent<NavMeshAgent>();
        navMeshSurface = GameObject.FindGameObjectWithTag("WalkableSurface").GetComponent<NavMeshSurface>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        cameraCenter = new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane);

        cameraCenter = mainCamera.ViewportToWorldPoint(cameraCenter);
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cameraCenter, 0.5f);
    }

    public override void EnterState()
    {
        agent.updateRotation = false;
        destination = GetFurthestPoint();
        agent.SetDestination(destination);
        agent.speed = crazeSpeed;
    }

    public override void UpdateState()
    {
        if (IsObjectOutsideViewport(monoBehaviour.transform.position)){
            UnityEngine.Object.Destroy(monoBehaviour.gameObject);
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
    

    public Vector3 GetFurthestPoint(){
        Vector3 leftTop = GetLeftTopCorner();
        Vector3 leftBottom = GetLeftBottomCorner();
        Vector3 rightTop = GetRightTopCorner();
        Vector3 rightBottom = GetRightBottomCorner();

        float distanceLeftTop = Vector3.Distance(leftTop, cameraCenter);
        float distanceLeftBottom = Vector3.Distance(leftBottom, cameraCenter);
        float distanceRightTop = Vector3.Distance(rightTop, cameraCenter);
        float distanceRightBottom = Vector3.Distance(rightBottom, cameraCenter);

        if(distanceLeftTop > distanceLeftBottom && distanceLeftTop > distanceRightTop && distanceLeftTop > distanceRightBottom){
            return leftTop;
        } else if(distanceLeftBottom > distanceRightTop && distanceLeftBottom > distanceRightBottom){
            return leftBottom;
        } else if(distanceRightTop > distanceRightBottom){
            return rightTop;
        } else {
            return rightBottom;
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
