
using System;
using DG.Tweening;
using UnityEngine;

public class CatHidingState : CatBaseState {
    private Sequence sq;

    public CatHidingState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        stm.Agent.ResetPath();
        stm.Agent.enabled = false;
    }

    public void StartHiding(Vector3 position) {
        Sequence fadeSq = DOTween.Sequence();
        fadeSq.AppendInterval(stm.Hide.FadeDelay);
        fadeSq.Append(stm.Renderer.DOFade(0, stm.Hide.FadeDuration));

        sq = DOTween.Sequence();
        sq.Append(stm.transform.DOMove(position, stm.Hide.Duration).SetEase(Ease.InOutSine));
        sq.Join(fadeSq);

        sq.Play();
    }

    public void QuitHiding(Vector3 position, Action onComplete) {
        sq?.Kill(complete: true);

        stm.transform.position = position;
        stm.Renderer.DOFade(1, stm.Hide.FadeDuration).OnComplete(
        () => {
            stm.Agent.enabled = true;
            onComplete?.Invoke();
        });
    }

    public override void ExitState() {
    }
}