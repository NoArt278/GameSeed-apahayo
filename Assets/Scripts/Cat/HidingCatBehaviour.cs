using System;
using DG.Tweening;
using UnityEngine;

public class HidingCatBehaviour : MonoBehaviour {
    private float fadeDelay = 0.0f;
    private float fadeDuration = 0.2f;
    private float hideDuration = 0.5f;

    private CatBehaviourManager cbm;

    private void Awake() {
        cbm = GetComponent<CatBehaviourManager>();
    }

    public void StartHiding(Vector3 hidePosition) {
        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.AppendInterval(fadeDelay);
        fadeSequence.Append(cbm.CatRenderer.DOFade(0, fadeDuration));

        Sequence wholeSequence = DOTween.Sequence();
        wholeSequence.Append(transform.DOMove(hidePosition, hideDuration).SetEase(Ease.InOutSine));
        wholeSequence.Join(fadeSequence);

        wholeSequence.Play();
    }

    public void QuitHiding(Vector3 quitPosition, Action onQuitComplete) {
        transform.position = quitPosition;
        cbm.CatRenderer.DOFade(1, fadeDuration).OnComplete(() => onQuitComplete());
    }
}