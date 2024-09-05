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

    private HypnotizeUIManager barUI;

    public NPCHypnotizedState(NPCStateMachine stm) : base(stm)
    {
        agent = stm.Agent;
        barUI = stm.BarUI;
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

        barUI.EnableHypnoBar();
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
                stm.TransitionToState(stm.STATE_CRAZE);
            }
        }

        barUI.UpdateHypnoBar(currentHypnotizeValue / hypnotizeHealth);
    }

    private void TimerUpdate() {
        if (timer >= maxHypnotizeDelay) {
            stm.TransitionToState(stm.STATE_RANDOMMOVE);
        }

    }

    public override void ExitState()
    {
        barUI.DisableHypnoBar();
        agent.speed = initialSpeed;
    }
}
