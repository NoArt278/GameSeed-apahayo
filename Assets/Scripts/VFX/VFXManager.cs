using System.Collections;
using UnityEngine;

public class VFXManager : SingletonMB<VFXManager> {
    [SerializeField] private ParticleSystem poofVFX;
    [SerializeField] private Animator zapVFX;

    private Transform zapVFXTransform;

    public void PlayPoofVFX(Vector3 position) {
        poofVFX.transform.position = position;
        poofVFX.Play();
        AudioManager.Instance.PlayOneShot("Poof");
    }

    public void PlayZapVFX(Transform tr) {
        zapVFX.transform.position = tr.position;
        zapVFX.gameObject.SetActive(true);
        zapVFXTransform = tr;
        StartCoroutine(ZapVFXRoutine());
    }

    private IEnumerator ZapVFXRoutine() {
        while (zapVFX.gameObject.activeSelf) {
            zapVFX.transform.position = zapVFXTransform.position;
            yield return null;
        }
    }

    public void StopZapVFX() {
        zapVFX.gameObject.SetActive(false);
    }
}