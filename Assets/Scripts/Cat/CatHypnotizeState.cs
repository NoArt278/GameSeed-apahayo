using DG.Tweening;
using UnityEngine;

public class CatHypnotizeState : CatBaseState {
    public CatHypnotizeState(CatStateMachine stm) : base(stm) { }
    private Tween moveTween;

    public void StartHypnotize(Vector3 floatPosition) {
        stm.ChangeState(stm.STATE_HYPNOTIZE);
        stm.Agent.enabled = false;

        float duration = 0.2f;
        stm.Renderer.flipX = floatPosition.x < stm.transform.position.x;
        stm.Animator.SetBool("Float", true);
        stm.FloatVFX.gameObject.SetActive(true);
        moveTween?.Kill();
        moveTween = stm.transform.DOMove(floatPosition, duration).SetEase(Ease.OutQuad);
    }

    public void CancelHypnotize(Vector3 backPosition) {
        float duration = 0.2f;
        moveTween?.Kill();
        stm.FloatVFX.gameObject.SetActive(false);
        moveTween = stm.transform.DOMove(backPosition, duration).SetEase(Ease.OutQuad).OnComplete(
            () => {
                stm.Animator.SetBool("Float", false);
                stm.Agent.enabled = true;
                stm.ChangeState(stm.STATE_FOLLOW);
            }
        );
    }
}