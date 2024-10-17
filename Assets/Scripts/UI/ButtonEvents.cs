using DG.Tweening;
using UnityEngine;

public class ButtonEvents : MonoBehaviour {
    private float _targetScale;

    public void PlayHoverSound() {
        AudioManager.Instance.PlayOneShot("Hover");
    }

    public void PlayClickSound() {
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void SetTargetScale(float targetScale) {
        _targetScale = targetScale;
    }

    public void ScaleTr(Transform obj) {
        obj.DOScale(new Vector3(_targetScale, _targetScale, _targetScale), 0.1f).SetUpdate(true);
    }
}