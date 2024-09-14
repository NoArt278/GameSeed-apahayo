using UnityEngine;

public class VFXManager : MonoBehaviour {
    public static VFXManager Instance { get; private set; }
    [SerializeField] private ParticleSystem poofVFX;
    [SerializeField] private Animator zapVFX;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void PlayPoofVFX(Vector3 position) {
        poofVFX.transform.position = position;
        poofVFX.Play();
    }

    public void PlayZapVFX(Vector3 position) {
        zapVFX.transform.position = position;
        zapVFX.gameObject.SetActive(true);
    }

    public void StopZapVFX() {
        zapVFX.gameObject.SetActive(false);
    }
}