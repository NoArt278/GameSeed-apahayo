
using System;
using DG.Tweening;
using UnityEngine;

public class CatHidingState : CatBaseState {
    private Sequence _sq;
    public CatHidingState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        STM.Agent.ResetPath();
        STM.Agent.enabled = false;
    }

    public void StartHiding(Vector3 position) {
        Sequence fadeSq = DOTween.Sequence();
        fadeSq.AppendInterval(STM.Hide.FadeDelay);
        fadeSq.Append(STM.Renderer.DOFade(0, STM.Hide.FadeDuration));

        _sq = DOTween.Sequence();
        _sq.Append(STM.transform.DOMove(position, STM.Hide.Duration).SetEase(Ease.InOutSine));
        _sq.Join(fadeSq);

        _sq.Play();
    }

    public void QuitHiding(Vector3 position, Action onComplete) {
        _sq?.Kill(complete: true);

        STM.transform.position = position;
        STM.Renderer.DOFade(1, STM.Hide.FadeDuration).OnComplete(
        () => {
            STM.Agent.enabled = true;
            onComplete?.Invoke();
        });
    }

    public override void ExitState() {
    }
}