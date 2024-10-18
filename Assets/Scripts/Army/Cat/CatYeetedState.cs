using DG.Tweening;
using UnityEngine;

public class CatYeetedState : CatBaseState {
    public CatYeetedState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        STM.Animator.SetTrigger("Yeeted");
        AudioManager.Instance.PlayOneShot("Yeet");
    }

    public void Yeeted(Vector3 yeetPosition) {
        float duration = 0.9f;
        STM.Renderer.DOFade(0, duration);
        STM.transform.DOJump(yeetPosition, 1f, 1, duration).SetEase(Ease.Linear).OnComplete(
            () => {
                STM.YeetedCallback();
            }
        );
    }   
}
