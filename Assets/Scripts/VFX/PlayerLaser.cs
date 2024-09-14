using UnityEngine;

public class PlayerLaser : MonoBehaviour {
    [SerializeField] private SpriteRenderer crystal;
    [SerializeField] private SpriteRenderer player;
    [SerializeField] private LineRenderer lineRenderer;

    private bool laserActive;
    private Transform from;
    private Transform to;

    public void InitiateCrystal() {
        crystal.flipX = player.flipX;
        crystal.gameObject.SetActive(true);
    }
    
    public void EmitLaser(Transform from, Transform to) {
        this.from = from;
        this.to = to;
        laserActive = true;
        lineRenderer.enabled = true;

        VFXManager.Instance.PlayZapVFX(to.GetComponent<NPCStateMachine>().Center);
    }

    private void Update() {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        if (!laserActive) return;
        lineRenderer.SetPosition(0, from.position);
        lineRenderer.SetPosition(1, to.position + Vector3.up * 1f);
    }

    public void StopLaser() {
        if (!laserActive) return;
        crystal.gameObject.SetActive(false);
        laserActive = false;
        lineRenderer.enabled = false;

        VFXManager.Instance.StopZapVFX();
    }
}