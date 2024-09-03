using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ArmyCatBehaviour : MonoBehaviour {
    [Header("References")]
    [SerializeField] private SpriteRenderer catRenderer;

    [Header("Properties")]
    private Transform follow;
    private NavMeshAgent agent;
    private float followSpeed;

    private bool followTheLeader = true;
    private bool onAction = false;
    private Vector3 towardsFleePosition;
    private Vector3 fleePosition;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Transform follow, float followSpeed) {
        this.follow = follow;
        this.followSpeed = followSpeed;

        agent.speed = followSpeed;
        agent.SetDestination(follow.position);
        agent.velocity = Vector3.zero;
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
        agent.speed = followSpeed;
    }

    public void Sprint(float speed) {
        agent.speed = speed;
    }

    public void StopSprint() {
        agent.speed = followSpeed;
    }

    private void Update() {
        if (!onAction) Follow();
        if (agent.isOnOffMeshLink && !onAction) StartJumpToPlatform();

        AlignOrientation();
    }

    private void Follow() {
        if (followTheLeader) {
            if (follow != null) agent.SetDestination(follow.position);
        } else {
            towardsFleePosition = Vector3.MoveTowards(towardsFleePosition, fleePosition, 0.5f);
            agent.SetDestination(towardsFleePosition);
        }
    }

    public void Flee() {
        followTheLeader = false;

        Vector3 fleeDirection = (transform.position - follow.position).normalized;
        fleePosition = follow.position + 100f * agent.radius * fleeDirection;
        towardsFleePosition = transform.position;

        Debug.DrawLine(transform.position, fleePosition, Color.red, 5f);

        agent.SetDestination(towardsFleePosition);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1.2f);
        sequence.Append(catRenderer.DOFade(0, 0.4f));

        sequence.Play();
    }

    private void AlignOrientation() {
        if (agent.velocity.sqrMagnitude > 0.1f) catRenderer.flipX = agent.velocity.x < 0;
    }

    private void StartJumpToPlatform() {
        onAction = true;
        NavMeshLink link = agent.navMeshOwner as NavMeshLink;
        bool reverse = CheckIfJumpingFromEndToStart(link);
        Spline spline = reverse ? link.GetComponent<NavMeshLinkSpline>().SplineDrop : link.GetComponent<NavMeshLinkSpline>().SplineJump;
        StartCoroutine(JumpToPlatformCoroutine(spline, reverse));

        // OnStartJump?.Invoke();
    }


    private IEnumerator JumpToPlatformCoroutine(Spline spline, bool reverseDirection) {
        float currentTime = 0;
        float jumpDuration = 0.2f;
        Vector3 agentStartPosition = agent.transform.position;

        while (currentTime < jumpDuration)
        {
            currentTime += Time.deltaTime;

            float amount = Mathf.Clamp01(currentTime / jumpDuration);
            amount = reverseDirection ? 1 - amount : amount;

            agent.transform.position =
                reverseDirection ?
                spline.CalculatePositionCustomEnd(amount, agentStartPosition)
                : spline.CalculatePositionCustomStart(amount, agentStartPosition);

            yield return new WaitForEndOfFrame();
        }

        agent.CompleteOffMeshLink();

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
            = Vector3.Distance(agent.transform.position, startPosWorld);
        float distancePlayerToEnd 
            = Vector3.Distance(agent.transform.position, endPosWorld);


        return distancePlayerToStart > distancePlayerToEnd;
    }

    #if UNITY_EDITOR
    [Button]
    private void LookAtCamera() {
        if (Camera.main != null) {
            Vector3 direction = Camera.main.transform.position - transform.position;
            float xAngle = Mathf.Atan2(direction.y, -direction.z) * Mathf.Rad2Deg;
            float yAngle = Mathf.Atan2(-direction.x, -direction.z) * Mathf.Rad2Deg;

            catRenderer.transform.rotation = Quaternion.Euler(xAngle, yAngle, 0);
        } else {
            Debug.LogWarning("Main camera not found");
        }
    }
    #endif
}