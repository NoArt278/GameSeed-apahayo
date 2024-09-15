using DG.Tweening;
using UnityEngine;

public class CatHypnotizeState : CatBaseState {
    public CatHypnotizeState(CatStateMachine stm) : base(stm) { }
    private Tween moveTween;
    public bool OnAnimation = false;

    public void StartHypnotize(Vector3 floatPosition) {
        if (OnAnimation) return;
        OnAnimation = true;
        stm.ChangeState(stm.STATE_HYPNOTIZE);
        stm.Agent.enabled = false;

        float duration = 0.3f;
        stm.Renderer.flipX = floatPosition.x < stm.transform.position.x;
        stm.Animator.SetBool("Float", true);
        stm.FloatVFX.gameObject.SetActive(true);
        moveTween?.Kill();
        moveTween = stm.transform.DOMove(floatPosition, duration).SetEase(Ease.OutQuad).OnComplete(
            () => {
                OnAnimation = false;
                moveTween = null;
            }
        );
    }

    public void CancelHypnotize(Vector3 backPosition) {
        if (OnAnimation) return;
        OnAnimation = true;
        float duration = 0.3f;
        moveTween?.Kill();
        stm.FloatVFX.gameObject.SetActive(false);
        moveTween = stm.transform.DOMove(backPosition, duration).SetEase(Ease.OutQuad).OnComplete(
            () => {
                stm.Animator.SetBool("Float", false);
                stm.Agent.enabled = true;
                stm.Agent.Warp(backPosition);
                stm.ChangeState(stm.STATE_FOLLOW);
                OnAnimation = false;
                moveTween = null;
            }
        );
    }

    public void ResetCat() {
        stm.Animator.SetTrigger("Reset");
        stm.Agent.enabled = true;
        stm.FloatVFX.gameObject.SetActive(false);
        OnAnimation = false;
        stm.Follow.Target = null;
        moveTween = null;
    }
}