using UnityEngine;
using UnityEngine.AI;

public class NPCHypnotizedState : NPCBaseState
{
    private readonly NavMeshAgent agent;
    private float hypnotizeHealth = 10f;
    private float currentHypnotizeValue = 0f;

    private float timer;
    private float maxHypnotizeDelay = 2f;
    private float initialSpeed;

    public NPCHypnotizedState(NPCStateMachine stm) : base(stm)
    {
        agent = stm.Agent;
    }

    public void SetHypnotizeStats(HypnotizeStats stats) {
        hypnotizeHealth = stats.hypnotizeHealth;
        maxHypnotizeDelay = stats.maxHypnotizeDelay;
    }

    public override void EnterState()
    {
        initialSpeed = agent.speed;
        agent.speed = 0;
        currentHypnotizeValue = 1f;
        timer = 0f;

        stm.animator.SetBool("isHypno", true);

        stm.SpriteRenderer.color = Color.blue;
    }

    public override void UpdateState()
    {
        TimerUpdate();
        MeterUpdate();

        timer += Time.deltaTime;
    }

    private void MeterUpdate() {
        if (stm.IsNPCClicked()) {
            timer = 0f;
            currentHypnotizeValue += 1f;
            if (hypnotizeHealth <= currentHypnotizeValue) {
                stm.TransitionToState(stm.STATE_WANDER);
            }
        }
        if (stm.isControllingBar)
        {
            GameplayUI.Instance.UpdateHypnoBar(currentHypnotizeValue / hypnotizeHealth);
        }
    }

    private void TimerUpdate() {
        if (timer >= maxHypnotizeDelay) {
            stm.animator.SetBool("isHypno", false);
            stm.TransitionToState(stm.STATE_RANDOMMOVE);
        }

    }

    public override void ExitState()
    {
        agent.speed = initialSpeed;
    }
}
