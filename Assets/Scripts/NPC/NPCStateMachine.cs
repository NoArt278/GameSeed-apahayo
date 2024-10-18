using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class HypnotizeStats
{
    public float hypnotizeHealth = 10f;
    public float maxHypnotizeDelay = 2f;
}

public class NPCStateMachine : MonoBehaviour
{
    public NPCBaseState CurrentState;
    [ShowNativeProperty] public string CurrentStateName => CurrentState != null ? CurrentState.GetType().Name : "None";

    // STATES ========================================
    public  NPCIdleState        STATE_IDLE { get; private set; }
    public  NPCRandomMoveState  STATE_RANDOMMOVE { get; private set; }
    public  NPCWayPointState    STATE_WAYPOINT { get; private set; }
    public  NPCHypnotizedState  STATE_HYPNOTIZED { get; private set; }
    public  NPCCrazeState       STATE_CRAZE { get; private set; }
    public  NPCWanderState       STATE_WANDER { get; private set; }

    // COMPONENTS ====================================
    public  NavMeshAgent        Agent { get; private set; }
    public  NavMeshSurface      Surface { get; private set; }
    public  SpriteRenderer      SpriteRenderer { get; private set; }
    public  Collider            Collider { get; private set; }

    // STATS =========================================
    [SerializeField] private HypnotizeStats _hypnotizeStats;
    public  HypnotizeStats HypnotizeStats { get => _hypnotizeStats; }
    [HideInInspector] public bool IsControllingBar = false;
    [HideInInspector] public bool IsCrazed = false;

    // SPAWNER =======================================
    [HideInInspector] public NPCSpawner Spawner;

    // ANIMATION =====================================
    [HideInInspector] public Animator Animator;
    public Transform Center;

    private void Awake() {
        // COMPONENTS
        Agent = GetComponent<NavMeshAgent>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Collider = GetComponent<Collider>();

        // STATES
        STATE_IDLE = new NPCIdleState(this);
        STATE_RANDOMMOVE = new NPCRandomMoveState(this);

        STATE_HYPNOTIZED = new NPCHypnotizedState(this);
        STATE_HYPNOTIZED.SetHypnotizeStats(HypnotizeStats);

        STATE_CRAZE = new NPCCrazeState(this);
        STATE_WANDER = new NPCWanderState(this);
        // STATE_WAYPOINT = new NPCWayPointState(this);

        // ANIMATION
        Animator = GetComponentInChildren<Animator>();

    }

    public void Initialize(NavMeshSurface surface, HypnotizeStats stats = null) {
        Surface = surface;
        if (stats != null) _hypnotizeStats = stats;
    }

    void Start()
    {
        CurrentState = STATE_RANDOMMOVE;
        CurrentState.EnterState();
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        CurrentState.UpdateState();

        if (IsCrazed && IsOutOfCamera()) SelfDestroy();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        IsNPCWalking();
    }    

    public bool IsOutOfCamera() {
        Vector2 clipSpace = Camera.main.WorldToViewportPoint(transform.position + SpriteRenderer.bounds.extents.y * Vector3.up);
        if (clipSpace.x < 0 || clipSpace.x > 1 || clipSpace.y < 0 || clipSpace.y > 1)
        {
            return true;
        }
        return false;
    }

    public void TransitionToState(NPCBaseState state)
    {
        CurrentState.ExitState();
        CurrentState = state;
        CurrentState.EnterState();
    }

    public void StartHyponotize()
    {
        TransitionToState(STATE_HYPNOTIZED);
    }

    public bool CheckHypnotize()
    {
        return CurrentState == STATE_HYPNOTIZED;
    }

    public bool CheckCrazed()
    {
        return CurrentState == STATE_WANDER;
    }

    public void OnNPCClicked()
    {
        if (CurrentState == STATE_HYPNOTIZED)
        {
            STATE_HYPNOTIZED.NPCClicked();
        }
    }

    public void IsNPCWalking(){
        Animator.SetBool("isWalking", Agent.velocity.sqrMagnitude > 0);
    }

    public void SelfDestroy() {
        Animator.SetTrigger("Reset");
        if (Spawner != null) Spawner.Return(gameObject);
        else Destroy(gameObject);
    }
}
