
using System;
using DG.Tweening;
using UnityEngine;

public class CatHidingState : CatBaseState {
    public CatHidingState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        stm.Agent.ResetPath();
        stm.Agent.enabled = false;
    }

    public void StartHiding(Vector3 position) {
        Sequence fadeSq = DOTween.Sequence();
        fadeSq.AppendInterval(stm.Hide.FadeDelay);
        fadeSq.Append(stm.Renderer.DOFade(0, stm.Hide.FadeDuration));

        Sequence wholeSq = DOTween.Sequence();
        wholeSq.Append(stm.transform.DOMove(position, stm.Hide.Duration).SetEase(Ease.InOutSine));
        wholeSq.Join(fadeSq);

        wholeSq.Play();
    }

    public void QuitHiding(Vector3 position, Action onComplete) {
        stm.transform.position = position;
        stm.Renderer.DOFade(1, stm.Hide.FadeDuration).OnComplete(() => onComplete());
    }

    public override void ExitState() {
        stm.Agent.enabled = true;
    }
}