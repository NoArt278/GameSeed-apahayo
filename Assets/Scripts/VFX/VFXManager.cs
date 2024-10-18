using System.Collections;
using UnityEngine;

public class VFXManager : SingletonMB<VFXManager> {
    [SerializeField] private ParticleSystem _poofVFX;
    [SerializeField] private Animator _zapVFX;

    private Transform _zapVFXTransform;

    public void PlayPoofVFX(Vector3 position) {
        _poofVFX.transform.position = position;
        _poofVFX.Play();
        AudioManager.Instance.PlayOneShot("Poof");
    }

    public void PlayZapVFX(Transform tr) {
        _zapVFX.transform.position = tr.position;
        _zapVFX.gameObject.SetActive(true);
        _zapVFXTransform = tr;
        StartCoroutine(ZapVFXRoutine());
    }

    private IEnumerator ZapVFXRoutine() {
        while (_zapVFX.gameObject.activeSelf) {
            _zapVFX.transform.position = _zapVFXTransform.position;
            yield return null;
        }
    }

    public void StopZapVFX() {
        _zapVFX.gameObject.SetActive(false);
    }
}