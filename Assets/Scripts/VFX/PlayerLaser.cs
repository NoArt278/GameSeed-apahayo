using UnityEngine;

public class PlayerLaser : MonoBehaviour {
    [SerializeField] private SpriteRenderer _crystal;
    [SerializeField] private SpriteRenderer _player;
    [SerializeField] private LineRenderer _lineRenderer;

    private bool _laserActive;
    private Transform _from;
    private Transform _to;

    public void InitiateCrystal() {
        _crystal.flipX = _player.flipX;
        _crystal.gameObject.SetActive(true);
    }
    
    public void EmitLaser(Transform from, Transform to) {
        this._from = from;
        this._to = to;
        _laserActive = true;
        _lineRenderer.enabled = true;

        VFXManager.Instance.PlayZapVFX(to.GetComponent<NPCStateMachine>().Center);
    }

    private void Update() {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        if (!_laserActive) return;
        _lineRenderer.SetPosition(0, _from.position);
        _lineRenderer.SetPosition(1, _to.position + Vector3.up * 1f);
    }

    public void StopLaser() {
        if (!_laserActive) return;
        _crystal.gameObject.SetActive(false);
        _laserActive = false;
        _lineRenderer.enabled = false;

        VFXManager.Instance.StopZapVFX();
    }
}