using System.Collections;
using UnityEngine;

public class CatStrayIdleState : CatBaseState {
    private Coroutine idleCoroutine;
    public CatStrayIdleState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        stm.Agent.ResetPath();
        idleCoroutine = stm.StartCoroutine(Idle(stm.Idle.Duration.RandomValue()));
    }

    private IEnumerator Idle(float duration) {
        yield return new WaitForSeconds(duration);
        stm.ChangeState(stm.STATE_STRAYWANDER);
    }

    public override void ExitState() {
        if (idleCoroutine != null) {
            stm.StopCoroutine(idleCoroutine);
        }
    }
}