using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class ArmyCatBehaviour : MonoBehaviour {
    [Header("References")]
    [SerializeField] private SpriteRenderer catRenderer;

    [Header("Properties")]
    // [SerializeField] private float rotationSpeed = 5f;
    private Transform follow;
    private NavMeshAgent agent;

    private bool followTheLeader = true;
    private Vector3 towardsFleePosition;
    private Vector3 fleePosition;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Transform follow) {
        this.follow = follow;
    }

    private void Update() {
        if (followTheLeader) {
            if (follow != null)
                agent.SetDestination(follow.position);
        } else {
            towardsFleePosition = Vector3.MoveTowards(towardsFleePosition, fleePosition, 0.5f);
            agent.SetDestination(towardsFleePosition);
        }

        AlignOrientation();
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
        // Vector3 direction = follow.position - transform.position;
        // float targetYAngle = direction.x > 0 ? 0 : 180;

        // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetYAngle, 0), Time.deltaTime * 5);
        // LookAtCamera();
        catRenderer.flipX = agent.velocity.x < 0;
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