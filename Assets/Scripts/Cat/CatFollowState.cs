using System.Collections;
using DG.Tweening;
using Unity.AI.Navigation;
using UnityEngine;

public class CatFollowState : CatBaseState {
    private bool onAction = false;
    private bool isSprinting = false;

    public CatFollowState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        stm.Agent.speed = stm.Follow.BaseSpeed;
        stm.Agent.SetDestination(stm.Follow.Target.position);
        stm.Agent.velocity = Vector3.zero;

        LittleJumpOnRegistered();
    }

    private void LittleJumpOnRegistered() {
        onAction = true;
        float jumpDuration = 0.4f;
        float jumpHeight = 0.4f;

        Vector3 ground = stm.transform.position;

        stm.transform.DOJump(ground, jumpHeight, 1, jumpDuration).OnComplete(() => onAction = false);
    }

    public void StartSprint(float speed) {
        stm.Agent.speed = speed;
        stm.Animator.speed = speed / stm.Follow.BaseSpeed;
        isSprinting = true;
    }

    public void StopSprint() {
        stm.Agent.speed = stm.Follow.BaseSpeed;
        stm.Animator.speed = 1;
        isSprinting = false;
    }

    public override void UpdateState() {
        if (!onAction) Follow();
        if (stm.Agent.isOnOffMeshLink && !onAction) StartJumpToPlatform();

        stm.AlignOrientation();
    }

    private void Follow() {
        if (stm.Follow.Target == null) {
            stm.ChangeState(stm.STATE_IDLE);
            return;
        }

        float speed = isSprinting ? stm.Follow.SprintSpeed : stm.Follow.BaseSpeed;
        speed += stm.Follow.SpeedDeviation.RandomValue();
        stm.Agent.speed = speed;
    }

    private void StartJumpToPlatform() {
        onAction = true;
        NavMeshLink link = stm.Agent.navMeshOwner as NavMeshLink;
        bool reverse = CheckIfJumpingFromEndToStart(link);
        Spline spline = reverse ? link.GetComponent<NavMeshLinkSpline>().SplineDrop : link.GetComponent<NavMeshLinkSpline>().SplineJump;
        if (spline == null) 
        {
            onAction = false;
        }

        stm.StartCoroutine(JumpToPlatformCoroutine(spline, reverse));
    }


    private IEnumerator JumpToPlatformCoroutine(Spline spline, bool reverseDirection) {
        float currentTime = 0;
        float jumpDuration = 0.2f;
        Vector3 agentStartPosition = stm.Agent.transform.position;

        Vector3 direction = spline.Direction;
        stm.Renderer.flipX = direction.x <= 0 ^ reverseDirection;

        while (currentTime < jumpDuration)
        {
            currentTime += Time.deltaTime;

            float amount = Mathf.Clamp01(currentTime / jumpDuration);
            amount = reverseDirection ? 1 - amount : amount;

            stm.Agent.transform.position =
                reverseDirection ?
                spline.CalculatePositionCustomEnd(amount, agentStartPosition)
                : spline.CalculatePositionCustomStart(amount, agentStartPosition);

            yield return new WaitForEndOfFrame();
        }

        stm.Agent.CompleteOffMeshLink();

        // OnLand?.Invoke();
        yield return new WaitForSeconds(0.1f);
        onAction = false;
    }

    private bool CheckIfJumpingFromEndToStart(NavMeshLink link)
    {
        Vector3 startPosWorld
            = link.gameObject.transform.TransformPoint(link.startPoint);
        Vector3 endPosWorld
            = link.gameObject.transform.TransformPoint(link.endPoint);

        float distancePlayerToStart 
            = Vector3.Distance(stm.Agent.transform.position, startPosWorld);
        float distancePlayerToEnd 
            = Vector3.Distance(stm.Agent.transform.position, endPosWorld);


        return distancePlayerToStart > distancePlayerToEnd;
    }
}