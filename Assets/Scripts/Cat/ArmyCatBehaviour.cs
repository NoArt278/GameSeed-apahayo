using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;

public class ArmyCatBehaviour : MonoBehaviour {
    [Header("Properties")]
    [SerializeField] private RangeFloat speedDeviation = new(-0.3f, 0.3f);
    private Transform follow;
    private float followSpeed;
    private float sprintSpeed;

    private bool followTheLeader = true;
    private bool onAction = false;
    private Vector3 towardsFleePosition;
    private Vector3 fleePosition;
    private bool isSprinting = false;
    private CatBehaviourManager cbm;

    private void Awake() {
        cbm = GetComponent<CatBehaviourManager>();
    }

    public void Initialize(Transform follow, float followSpeed) {
        this.follow = follow;
        this.followSpeed = followSpeed;

        cbm.Agent.speed = followSpeed;
        cbm.Agent.SetDestination(follow.position);
        cbm.Agent.velocity = Vector3.zero;
        LittleJumpOnRegistered();
    }

    private void LittleJumpOnRegistered() {
        onAction = true;
        float jumpDuration = 0.4f;
        float jumpHeight = 0.4f;

        Vector3 ground = transform.position;

        transform.DOJump(ground, jumpHeight, 1, jumpDuration).OnComplete(() => onAction = false);
    }
    
    private void OnEnable() {
        cbm.Agent.speed = followSpeed;
    }

    public void Sprint(float speed) {
        sprintSpeed = speed;
        cbm.Animator.speed = sprintSpeed / followSpeed;
        isSprinting = true;
    }

    public void StopSprint() {
        cbm.Animator.speed = 1;
        isSprinting = false;
    }

    private void Update() {
        if (!onAction) Follow();
        if (cbm.Agent.isOnOffMeshLink && !onAction) StartJumpToPlatform();

        AlignOrientation();
    }

    private void Follow() {
        if (followTheLeader) {
            if (follow != null) {
                float baseSpeed = isSprinting ? sprintSpeed : followSpeed;
                cbm.Agent.speed = baseSpeed + speedDeviation.RandomValue();
                cbm.Agent.SetDestination(follow.position);
            }
        } else {
            towardsFleePosition = Vector3.MoveTowards(towardsFleePosition, fleePosition, 0.5f);
            cbm.Agent.SetDestination(towardsFleePosition);
        }
    }

    public void Flee() {
        followTheLeader = false;

        Vector3 fleeDirection = (transform.position - follow.position).normalized;
        fleePosition = follow.position + 100f * cbm.Agent.radius * fleeDirection;
        towardsFleePosition = transform.position;

        Debug.DrawLine(transform.position, fleePosition, Color.red, 5f);

        cbm.Agent.SetDestination(towardsFleePosition);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1.2f);
        sequence.Append(cbm.CatRenderer.DOFade(0, 0.4f));

        sequence.Play();
    }

    private void AlignOrientation() {
        if (cbm.Agent.velocity.sqrMagnitude > 0.1f) cbm.CatRenderer.flipX = cbm.Agent.velocity.x < 0;
    }

    private void StartJumpToPlatform() {
        onAction = true;
        NavMeshLink link = cbm.Agent.navMeshOwner as NavMeshLink;
        bool reverse = CheckIfJumpingFromEndToStart(link);
        Spline spline = reverse ? link.GetComponent<NavMeshLinkSpline>().SplineDrop : link.GetComponent<NavMeshLinkSpline>().SplineJump;
        if (spline == null) 
        {
            onAction = false;
        }

        StartCoroutine(JumpToPlatformCoroutine(spline, reverse));
    }


    private IEnumerator JumpToPlatformCoroutine(Spline spline, bool reverseDirection) {
        float currentTime = 0;
        float jumpDuration = 0.2f;
        Vector3 agentStartPosition = cbm.Agent.transform.position;

        Vector3 direction = spline.Direction;
        cbm.CatRenderer.flipX = direction.x <= 0 ^ reverseDirection;

        while (currentTime < jumpDuration)
        {
            currentTime += Time.deltaTime;

            float amount = Mathf.Clamp01(currentTime / jumpDuration);
            amount = reverseDirection ? 1 - amount : amount;

            cbm.Agent.transform.position =
                reverseDirection ?
                spline.CalculatePositionCustomEnd(amount, agentStartPosition)
                : spline.CalculatePositionCustomStart(amount, agentStartPosition);

            yield return new WaitForEndOfFrame();
        }

        cbm.Agent.CompleteOffMeshLink();

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
            = Vector3.Distance(cbm.Agent.transform.position, startPosWorld);
        float distancePlayerToEnd 
            = Vector3.Distance(cbm.Agent.transform.position, endPosWorld);


        return distancePlayerToStart > distancePlayerToEnd;
    }

    #if UNITY_EDITOR
    [Button]
    private void LookAtCamera() {
        if (Camera.main != null) {
            Vector3 direction = Camera.main.transform.position - transform.position;
            float xAngle = Mathf.Atan2(direction.y, -direction.z) * Mathf.Rad2Deg;
            float yAngle = Mathf.Atan2(-direction.x, -direction.z) * Mathf.Rad2Deg;

            cbm.CatRenderer.transform.rotation = Quaternion.Euler(xAngle, yAngle, 0);
        } else {
            Debug.LogWarning("Main camera not found");
        }
    }
    #endif
}