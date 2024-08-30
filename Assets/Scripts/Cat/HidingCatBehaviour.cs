using System;
using DG.Tweening;
using UnityEngine;

public class HidingCatBehaviour : MonoBehaviour {
    [SerializeField] private float fadeDelay = 0.2f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float hideDuration = 0.5f;
    [SerializeField] private SpriteRenderer catRenderer;

    private Vector3 initialPosition;

    public void StartHiding(Vector3 hidePosition) {
        initialPosition = transform.position;

        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.AppendInterval(fadeDelay);
        fadeSequence.Append(catRenderer.DOFade(0, fadeDuration));

        Sequence wholeSequence = DOTween.Sequence();
        wholeSequence.Append(transform.DOMove(hidePosition, hideDuration).SetEase(Ease.InOutSine));
        wholeSequence.Join(fadeSequence);

        wholeSequence.Play();
    }

    public void QuitHiding(Action onQuitComplete) {
        transform.position = initialPosition;
        catRenderer.DOFade(1, fadeDuration).OnComplete(() => onQuitComplete());
    }
}