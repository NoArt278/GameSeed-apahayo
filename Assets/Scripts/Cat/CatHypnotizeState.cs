using DG.Tweening;
using UnityEngine;

public class CatHypnotizeState : CatBaseState {
    public CatHypnotizeState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        stm.Agent.enabled = false;
        // stm.Trail.gameObject.SetActive(true);
        stm.Animator.SetBool("Float", true);
    }

    public void StartHypnotize(Vector3 floatPosition) {
        stm.ChangeState(stm.STATE_HYPNOTIZE);

        float duration = 0.2f;
        stm.Renderer.flipX = floatPosition.x < stm.transform.position.x;
        stm.transform.DOMove(floatPosition, duration).SetEase(Ease.OutQuad);
    }

    public void CancelHypnotize(Vector3 backPosition) {
        float duration = 0.2f;
        stm.transform.DOMove(backPosition, duration).SetEase(Ease.OutQuad).OnComplete(() => stm.ChangeState(stm.STATE_FOLLOW));
    }

    public override void ExitState() {
        // stm.Trail.gameObject.SetActive(false);
        stm.Animator.SetBool("Float", false);
        stm.Agent.enabled = true;
    }
}