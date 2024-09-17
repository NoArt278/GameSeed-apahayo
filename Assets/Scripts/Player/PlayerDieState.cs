
using DG.Tweening;

public class PlayerDieState : PlayerBaseState {
    public PlayerDieState(PlayerStateMachine stm) : base(stm) { }

    public override void EnterState() {
        stm.Animator.SetTrigger("Die");

        AudioManager.Instance.PlayOneShot("Caught");
        AudioManager.Instance.PlayOneShot("Yeet");

        DOVirtual.DelayedCall(1f, () => {
            InGameScreen.Instance.ShowEndGameScreen();
        });
    }
}