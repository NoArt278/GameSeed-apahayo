using System.Collections;
using UnityEngine;

public class CatStrayIdleState : CatBaseState {
    private Coroutine _idleCoroutine;
    public CatStrayIdleState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        STM.Agent.ResetPath();
        _idleCoroutine = STM.StartCoroutine(Idle(STM.Idle.Duration.RandomValue()));
    }

    private IEnumerator Idle(float duration) {
        yield return new WaitForSeconds(duration);
        STM.ChangeState(STM.STATE_STRAYWANDER);
    }

    public override void ExitState() {
        if (_idleCoroutine != null) {
            STM.StopCoroutine(_idleCoroutine);
        }
    }
}