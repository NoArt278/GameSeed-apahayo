using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class CatStateMachine : MonoBehaviour {
    // PROPERTIES ====================================
    [Serializable]
    public class WanderProperties {
        public float Speed = 3.5f;
        public RangeFloat Radius = new(.5f, 2f);
    }

    [Serializable]
    public class IdleProperties {
        public RangeFloat Duration = new(1, 4);
    }

    [Serializable]
    public class HideProperties {
        public float FadeDelay = 0.0f;
        public float FadeDuration = 0.2f;
        public float Duration = 0.5f;
    }

    [Serializable]
    public class FollowProperties {
        public RangeFloat SpeedDeviation = new(-0.3f, 0.3f);
        public Transform Target;
        public float BaseSpeed;
        public float SprintSpeed;
    }

    public WanderProperties Wander;
    public IdleProperties Idle;
    public HideProperties Hide;
    [ReadOnly] public FollowProperties Follow;

    public CatBaseState PreviousState { get; private set; }
    public CatBaseState CurrentState { get; private set; }

    [ShowNativeProperty] public string CurrentStateName => CurrentState != null ? CurrentState.GetType().Name : "None";
    [ShowNativeProperty] public string PreviousStateName => PreviousState != null ? PreviousState.GetType().Name : "None";

    // STATES ========================================
    public  CatStrayIdleState   STATE_STRAYIDLE { get; private set; }
    public  CatStrayWanderState STATE_STRAYWANDER { get; private set; }
    public  CatFollowState      STATE_FOLLOW { get; private set; }
    public  CatHidingState      STATE_HIDING { get; private set; }
    public  CatHypnotizeState   STATE_HYPNOTIZE { get; private set; }

    // COMPONENTS ====================================
    // Reference
    [SerializeField] private SpriteRenderer catRenderer;
    [SerializeField] private Animator animator;

    // Getters
    public SpriteRenderer Renderer { get => catRenderer; }
    public Animator Animator { get => animator; }
    public NavMeshAgent Agent { get; private set; }

    public Action<CatBaseState, CatBaseState> OnStateChanged;
    [ReadOnly] public CatSpawner Spawner;

    private void Awake() {
        Agent = GetComponent<NavMeshAgent>();

        // STATES
        STATE_STRAYIDLE = new CatStrayIdleState(this);
        STATE_STRAYWANDER = new CatStrayWanderState(this);
        STATE_FOLLOW = new CatFollowState(this);
        STATE_HIDING = new CatHidingState(this);
        STATE_HYPNOTIZE = new CatHypnotizeState(this);
    }

    public void BecomeFollower(Transform target, float baseSpeed, float sprintSpeed) {
        Follow.Target = target;
        Follow.BaseSpeed = baseSpeed;
        Follow.SprintSpeed = sprintSpeed;

        ChangeState(STATE_FOLLOW);
    }

    public void GoHiding(Vector3 position) {
        ChangeState(STATE_HIDING);
        STATE_HIDING.StartHiding(position);
    }

    public void QuitHiding(Vector3 position) {
        STATE_HIDING.QuitHiding(position, () => ChangeState(PreviousState));
    }

    public void StartSprint() {
        STATE_FOLLOW.StartSprint(Follow.SprintSpeed);
    }

    public void StopSprint() {
        STATE_FOLLOW.StopSprint();
    }

    public void UseCatForHypnotize(Vector3 catFloatPos) {
        if (CurrentState == STATE_HYPNOTIZE) return;

        STATE_HYPNOTIZE.StartHypnotize(catFloatPos);
    }

    public void CancelHypnotize(Vector3 backPosition) {
        STATE_HYPNOTIZE.CancelHypnotize(backPosition);
    }

    private void Start() {
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < 0.5f) ChangeState(STATE_STRAYWANDER);
        else ChangeState(STATE_STRAYIDLE);
    }

    private void Update() {
        CurrentState?.UpdateState();

        animator.SetFloat("Speed", Agent.velocity.magnitude);
        animator.SetFloat("SignZ", Agent.velocity.z);
    }

    public void ChangeState(CatBaseState newState) {
        if (CurrentState == newState) return;

        PreviousState = CurrentState;

        CurrentState?.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();

        OnStateChanged?.Invoke(PreviousState, newState);
    }

    public void AlignOrientation() {
        if (Agent.velocity.sqrMagnitude > 0.1f) catRenderer.flipX = Agent.velocity.x < 0;
    }

    public void ReturnToSpawner() {
        if (Spawner) Spawner.Return(gameObject);
        else Destroy(gameObject);
    }
}