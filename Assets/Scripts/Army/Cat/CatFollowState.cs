using DG.Tweening;
using UnityEngine;

public class CatFollowState : CatBaseState {
    private bool onAction = false;
    private bool isSprinting = false;
    private bool firstFollow = true;

    public CatFollowState(CatStateMachine stm) : base(stm) { }

    public override void EnterState() {
        stm.Agent.speed = stm.Follow.BaseSpeed;
        bool success = stm.Agent.SetDestination(stm.Follow.Target.position);
        if (!success) {
            stm.Agent.Warp(stm.CatArmy.FindAppropriateSpawnLocation(stm.Follow.Target.position));
        }
        stm.Agent.velocity = Vector3.zero;
        stm.Animator.SetTrigger("Hypnotized");

        if (firstFollow) {
            AudioManager.Instance.PlayOneShot("Acquired");
            LittleJumpOnRegistered();
            firstFollow = false;
        }
    }

    public override void ExitState() {
        stm.Agent.ResetPath();
    }

    private void LittleJumpOnRegistered() {
        onAction = true;
        float jumpDuration = 0.4f;
        float jumpHeight = 0.4f;

        Vector3 ground = stm.transform.position;

        stm.transform.DOJump(ground, jumpHeight, 1, jumpDuration).OnComplete(() => onAction = false);
    }

    public void StartSprint(float speed) {
        stm.Agent.speed = speed;
        stm.Animator.speed = speed / stm.Follow.BaseSpeed;
        isSprinting = true;
    }

    public void StopSprint() {
        stm.Agent.speed = stm.Follow.BaseSpeed;
        stm.Animator.speed = 1;
        isSprinting = false;
    }

    public override void UpdateState() {
        if (!onAction) Follow();
        stm.AlignOrientation();
    }

    private void Follow() {
        float speed = isSprinting ? stm.Follow.SprintSpeed : stm.Follow.BaseSpeed;
        speed += stm.Follow.SpeedDeviation.RandomValue();
        stm.Agent.speed = speed;

        stm.Agent.SetDestination(stm.Follow.Target.position);
    }
}