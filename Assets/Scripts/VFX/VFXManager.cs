using UnityEngine;

public class VFXManager : MonoBehaviour {
    public static VFXManager Instance { get; private set; }
    [SerializeField] private ParticleSystem poofVFX;

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
}