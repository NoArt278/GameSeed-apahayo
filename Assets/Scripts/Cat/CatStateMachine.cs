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
    [HideInInspector] public FollowProperties Follow;

    [SerializeField, ReadOnly] private CatBaseState currentState;
    public CatBaseState CurrentState;
    // STATES ========================================
    public  CatIdleState        STATE_IDLE { get; private set; }
    public  CatWanderState      STATE_WANDER { get; private set; }
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
    public Action OnDestroyed;

    private void Awake() {
        Agent = GetComponent<NavMeshAgent>();

        // STATES
        STATE_IDLE = new CatIdleState(this);
        STATE_WANDER = new CatWanderState(this);
        STATE_FOLLOW = new CatFollowState(this);
        STATE_HIDING = new CatHidingState(this);
        STATE_HYPNOTIZE = new CatHypnotizeState(this);

        // Set initial state
        ChangeState(STATE_IDLE);
    }

    private void Start() {
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < 0.5f) ChangeState(STATE_WANDER);
        else ChangeState(STATE_IDLE);
    }

    private void Update() {
        currentState?.UpdateState();

        animator.SetFloat("Speed", Agent.velocity.magnitude);
        animator.SetFloat("SignZ", Agent.velocity.z);
    }

    public void ChangeState(CatBaseState newState) {
        if (currentState == newState) return;

        currentState?.UpdateState();
        currentState = newState;
        currentState.EnterState();

        OnStateChanged?.Invoke(currentState, newState);
    }

    public void AlignOrientation() {
        if (Agent.velocity.sqrMagnitude > 0.1f) catRenderer.flipX = Agent.velocity.x < 0;
    }

    private void OnDestroy() {
        OnDestroyed?.Invoke();
    }
}