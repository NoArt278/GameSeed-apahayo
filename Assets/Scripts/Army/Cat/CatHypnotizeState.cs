using DG.Tweening;
using UnityEngine;

public class CatHypnotizeState : CatBaseState {
    private Tween _moveTween;
    public bool OnAnimation { get; private set; } = false;

    public CatHypnotizeState(CatStateMachine stm) : base(stm) { }

    public void StartHypnotize(Vector3 floatPosition) {
        if (OnAnimation) return;
        OnAnimation = true;
        STM.ChangeState(STM.STATE_HYPNOTIZE);
        STM.Agent.enabled = false;

        float duration = 0.3f;
        STM.Renderer.flipX = floatPosition.x < STM.transform.position.x;
        STM.Animator.SetBool("Float", true);
        STM.FloatVFX.gameObject.SetActive(true);
        _moveTween?.Kill();
        _moveTween = STM.transform.DOMove(floatPosition, duration).SetEase(Ease.OutQuad).OnComplete(
            () => {
                OnAnimation = false;
                _moveTween = null;
            }
        );
    }

    public void CancelHypnotize(Vector3 backPosition) {
        if (OnAnimation) return;
        OnAnimation = true;
        float duration = 0.3f;
        _moveTween?.Kill();
        STM.FloatVFX.gameObject.SetActive(false);
        _moveTween = STM.transform.DOMove(backPosition, duration).SetEase(Ease.OutQuad).OnComplete(
            () => {
                STM.Animator.SetBool("Float", false);
                STM.Agent.enabled = true;
                STM.Agent.Warp(backPosition);
                STM.ChangeState(STM.STATE_FOLLOW);
                OnAnimation = false;
                _moveTween = null;
            }
        );
    }

    public void ResetCat() {
        STM.Animator.SetTrigger("Reset");
        STM.Agent.enabled = true;
        STM.FloatVFX.gameObject.SetActive(false);
        OnAnimation = false;
        STM.Follow.Target = null;
        _moveTween = null;
    }
}