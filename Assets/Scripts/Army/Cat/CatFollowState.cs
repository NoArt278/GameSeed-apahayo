using DG.Tweening;
using UnityEngine;

public class CatFollowState : CatBaseState {
    private bool _onAction = false;
    private bool _isSprinting = false;
    private bool _firstFollow = true;

    public CatFollowState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        STM.Agent.speed = STM.Follow.BaseSpeed;
        bool success = STM.Agent.SetDestination(STM.Follow.Target.position);
        if (!success) {
            STM.Agent.Warp(STM.CatArmy.FindAppropriateSpawnLocation(STM.Follow.Target.position));
        }
        STM.Agent.velocity = Vector3.zero;
        STM.Animator.SetTrigger("Hypnotized");

        if (_firstFollow) {
            AudioManager.Instance.PlayOneShot("Acquired");
            LittleJumpOnRegistered();
            _firstFollow = false;
        }
    }

    public override void ExitState() {
        STM.Agent.ResetPath();
    }

    private void LittleJumpOnRegistered() {
        _onAction = true;
        float jumpDuration = 0.4f;
        float jumpHeight = 0.4f;

        Vector3 ground = STM.transform.position;

        STM.transform.DOJump(ground, jumpHeight, 1, jumpDuration).OnComplete(() => _onAction = false);
    }

    public void StartSprint(float speed) {
        STM.Agent.speed = speed;
        STM.Animator.speed = speed / STM.Follow.BaseSpeed;
        _isSprinting = true;
    }

    public void StopSprint() {
        STM.Agent.speed = STM.Follow.BaseSpeed;
        STM.Animator.speed = 1;
        _isSprinting = false;
    }

    public override void UpdateState() {
        if (!_onAction) Follow();
        STM.AlignOrientation();
    }

    private void Follow() {
        float speed = _isSprinting ? STM.Follow.SprintSpeed : STM.Follow.BaseSpeed;
        speed += STM.Follow.SpeedDeviation.RandomValue();
        STM.Agent.speed = speed;

        STM.Agent.SetDestination(STM.Follow.Target.position);
    }
}