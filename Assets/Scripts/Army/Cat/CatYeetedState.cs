using DG.Tweening;
using UnityEngine;

public class CatYeetedState : CatBaseState {
    public CatYeetedState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        stm.Animator.SetTrigger("Yeeted");
        AudioManager.Instance.PlayOneShot("Yeet");
    }

    public void Yeeted(Vector3 yeetPosition) {
        float duration = 0.9f;
        stm.Renderer.DOFade(0, duration);
        stm.transform.DOJump(yeetPosition, 1f, 1, duration).SetEase(Ease.Linear).OnComplete(
            () => {
                stm.YeetedCallback();
            }
        );
    }   
}
