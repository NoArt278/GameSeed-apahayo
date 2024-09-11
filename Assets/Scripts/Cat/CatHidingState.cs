
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

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

        stm.Renderer.DOFade(1, stm.Hide.FadeDuration).OnComplete(
        () => {
            stm.Agent.enabled = true;
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) {
                stm.transform.position = hit.position;
            }
            onComplete?.Invoke();
        });
    }

    public override void ExitState() {
    }
}