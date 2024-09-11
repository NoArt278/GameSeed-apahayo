using DG.Tweening;
using UnityEngine;

public class CatHypnotizeState : CatBaseState {
    public CatHypnotizeState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        stm.Agent.enabled = false;
    }

    public void StartHypnotize(Vector3 floatPosition) {
        stm.ChangeState(stm.STATE_HYPNOTIZE);

        float duration = 0.2f;
        stm.transform.DOMove(floatPosition, duration).SetEase(Ease.OutQuad);
    }

    public void CancelHypnotize(Vector3 backPosition) {
        stm.ChangeState(stm.STATE_FOLLOW);
    }

    public override void ExitState() {
        stm.Agent.enabled = true;
    }
}