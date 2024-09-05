using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CatBehaviourManager))]
[RequireComponent(typeof(StrayCatBehaviour))]
[RequireComponent(typeof(HidingCatBehaviour))]
public class CatBehaviourManager : MonoBehaviour {
    private ArmyCatBehaviour armyCatBehaviour;
    private StrayCatBehaviour strayCatBehaviour;
    private HidingCatBehaviour hidingCatBehaviour;

    public enum State { Stray, Army, Hiding }
    public State CurrentState { get; private set; } = State.Stray;
    public Action<State, State> OnStateChanged;
    public Action OnDestroyed;

    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    private void Awake() {
        armyCatBehaviour = GetComponent<ArmyCatBehaviour>();
        strayCatBehaviour = GetComponent<StrayCatBehaviour>();
        hidingCatBehaviour = GetComponent<HidingCatBehaviour>();

        agent = GetComponent<NavMeshAgent>();

        BecomeStrayCat();
    }

    private void ChangeState(State newState) {
        if (CurrentState == newState) return;
        
        State previousState = CurrentState;
        CurrentState = newState;
        OnStateChanged?.Invoke(previousState, CurrentState);
    }

    public void BecomeStrayCat() {
        armyCatBehaviour.enabled = false;
        strayCatBehaviour.enabled = true;
        hidingCatBehaviour.enabled = false;

        ChangeState(State.Stray);
    }

    public void BecomeArmyCat() {
        armyCatBehaviour.enabled = true;
        strayCatBehaviour.enabled = false;
        hidingCatBehaviour.enabled = false;

        ChangeState(State.Army);
    }

    public void BecomeHidingCat() {
        armyCatBehaviour.enabled = false;
        strayCatBehaviour.enabled = false;
        hidingCatBehaviour.enabled = true;

        ChangeState(State.Hiding);
    }

    private void Update() {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        animator.SetFloat("SignZ", agent.velocity.z);
    }

    private void OnDestroy() {
        OnDestroyed?.Invoke();
    }
}