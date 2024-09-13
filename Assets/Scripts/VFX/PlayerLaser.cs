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
        Debug.Log("Emitting laser");
        this.from = from;
        this.to = to;
        laserActive = true;
    }

    private void Update() {
        if (!laserActive) return;
        lineRenderer.SetPosition(0, from.position);
        lineRenderer.SetPosition(1, to.position + Vector3.up * 1f);
    }

    public void StopLaser() {
        Debug.Log("Stopping laser");

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);

        crystal.gameObject.SetActive(false);
        laserActive = false;
        lineRenderer.enabled = false;
    }
}