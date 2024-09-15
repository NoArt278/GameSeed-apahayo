using System;
using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[System.Serializable]
public class HypnotizeStats
{
    public float hypnotizeHealth = 10f;
    public float maxHypnotizeDelay = 2f;
}

public class NPCStateMachine : MonoBehaviour
{
    public NPCBaseState currentState;
    [ShowNativeProperty] public string CurrentStateName => currentState != null ? currentState.GetType().Name : "None";

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
    [SerializeField] private HypnotizeStats hypnotizeStats;
    public  HypnotizeStats HypnotizeStats { get => hypnotizeStats; }
    public bool isControllingBar = false;
    public bool IsCrazed = false;

    // SPAWNER =======================================
    public NPCSpawner Spawner;

    // ANIMATION =====================================
    public Animator animator;
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
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        currentState.UpdateState();

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
        return currentState == STATE_WANDER;
    }

    public void OnNPCClicked()
    {
        if (currentState == STATE_HYPNOTIZED)
        {
            STATE_HYPNOTIZED.NPCClicked();
        }
    }

    public void IsNPCWalking(){
        animator.SetBool("isWalking", Agent.velocity.sqrMagnitude > 0);
    }

    public void SelfDestroy() {
        if (Spawner != null) Spawner.Return(gameObject);
        else Destroy(gameObject);
    }
}
