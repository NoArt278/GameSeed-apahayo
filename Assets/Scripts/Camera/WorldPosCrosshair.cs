using UnityEngine;

public class WorldPosCrosshair : MonoBehaviour {
    private Camera _mainCam;
    [SerializeField] private LayerMask _groundMask;

    private bool _initialized = false;

    public void Initialize(Camera cam) {
        _mainCam = cam;
        _initialized = true;
    }

    private void Update() {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        if (!_initialized) return;
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundMask)) {
            transform.position = hit.point;
        }
    }
}