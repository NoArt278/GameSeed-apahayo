using UnityEngine;
using UnityEngine.AI;

public class NPCHypnotizedState : NPCBaseState
{
    private readonly NavMeshAgent _agent;
    private float _hypnotizeHealth = 10f;
    private float _currentHypnotizeValue = 0f;

    private float _timer;
    private float _maxHypnotizeDelay = 2f;
    private float _initialSpeed;

    public NPCHypnotizedState(NPCStateMachine stm) : base(stm)
    {
        _agent = stm.Agent;
    }

    public void SetHypnotizeStats(HypnotizeStats stats) {
        _hypnotizeHealth = stats.hypnotizeHealth;
        _maxHypnotizeDelay = stats.maxHypnotizeDelay;
    }

    public override void EnterState()
    {
        _initialSpeed = _agent.speed;
        _agent.speed = 0;
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;

        _currentHypnotizeValue = 1f;
        _timer = 0f;

        STM.Animator.SetBool("isHypno", true);
    }

    public override void UpdateState()
    {
        TimerUpdate();
        MeterUpdate();

        _timer += Time.deltaTime;
    }

    private void MeterUpdate() {
        if (STM.IsControllingBar)
        {
            GameplayUI.Instance.UpdateHypnoBar(_currentHypnotizeValue / _hypnotizeHealth);
        }
    }

    public void NPCClicked() {
        _timer = 0f;
        _currentHypnotizeValue += 1f;
        if (_hypnotizeHealth <= _currentHypnotizeValue) {
            STM.Animator.SetBool("isCraze", true);
            STM.IsCrazed = true;
            STM.TransitionToState(STM.STATE_WANDER);
            VFXManager.Instance.PlayPoofVFX(STM.transform.position + Vector3.up * 0.5f);
        }
    }

    private void TimerUpdate() {
        if (_timer >= _maxHypnotizeDelay) {
            STM.Animator.SetBool("isHypno", false);
            STM.TransitionToState(STM.STATE_RANDOMMOVE);
        }

    }

    public override void ExitState()
    {
        _agent.speed = _initialSpeed;
    }
}
