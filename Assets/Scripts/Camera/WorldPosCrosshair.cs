using UnityEngine;

public class WorldPosCrosshair : MonoBehaviour {
    private Camera mainCam;
    [SerializeField] private LayerMask groundMask;

    private bool initialized = false;

    public void Initialize(Camera cam) {
        mainCam = cam;
        initialized = true;
    }

    private void Update() {
        if (!initialized) return;
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask)) {
            transform.position = hit.point;
        }
        // transform.position = mainCam.ScreenToWorldPoint(Input.mousePosition);
    }
}