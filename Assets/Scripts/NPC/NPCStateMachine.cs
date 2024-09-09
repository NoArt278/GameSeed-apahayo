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
    [SerializeField, ReadOnly] private NPCBaseState currentState;

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
    public  HypnotizeUIManager  BarUI { get; private set; }
    public  SpriteRenderer      SpriteRenderer { get; private set; }
    public  Collider            Collider { get; private set; }

    // STATS =========================================
    [SerializeField] private HypnotizeStats hypnotizeStats;
    public  HypnotizeStats HypnotizeStats { get => hypnotizeStats; }

    // SPAWNER =======================================
    public NPCSpawner spawner;

    // ANIMATION =====================================
    public Animator animator;

    // Waypoints
    // public WaypointType waypointType;
    // [SerializeField] public Waypoint[] waypoints;

    private void Awake() {
        // COMPONENTS
        BarUI = GetComponent<HypnotizeUIManager>();
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
        animator = GetComponentInChildren<Animator>();

    }

    public void Initialize(NavMeshSurface surface, HypnotizeStats stats = null) {
        Surface = surface;
        if (stats != null) hypnotizeStats = stats;
    }

    void Start()
    {
        currentState = STATE_RANDOMMOVE;
        currentState.EnterState();
    }

    void Update()
    {
        currentState.UpdateState();
    }

    public void TransitionToState(NPCBaseState state)
    {
        currentState.ExitState();
        currentState = state;
        currentState.EnterState();
    }

    public void StartHyponotize()
    {
        TransitionToState(STATE_HYPNOTIZED);
    }

    public bool CheckHypnotize()
    {
        return currentState == STATE_HYPNOTIZED;
    }

    public bool CheckCrazed()
    {
        return currentState == STATE_CRAZE;
    }

    public bool IsNPCClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("NPC")))
            {
                return hit.collider == Collider;
            }
        }
        return false;
    }

    public void IsNPCWalking(){
        // Debug.LogWarning(Agent.velocity.sqrMagnitude > 0.1f);
        animator.SetBool("isWalking", Agent.velocity.sqrMagnitude > 0);
    }
}
