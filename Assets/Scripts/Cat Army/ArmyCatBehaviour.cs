using NaughtyAttributes;
using UnityEngine;

public class ArmyCatBehaviour : MonoBehaviour {
    [Header("References")]
    [SerializeField] private SpriteRenderer catRenderer;

    [Header("Properties")]
    [SerializeField] private float followSpeed = 3f;
    private Transform follow;

    public void Initialize(Transform follow) {
        this.follow = follow;
    }

    private void StickToGround() {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f)) {
            transform.position = hit.point;
        }
    }

    private void Update() {
        StickToGround();
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