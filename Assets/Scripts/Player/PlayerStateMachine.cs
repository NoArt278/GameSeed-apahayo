using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour {

    public PlayerBaseState CurrentState { get; private set; }
    public PlayerBaseState PreviousState { get; private set; }

    [ShowNativeProperty] public string CurrentStateName => CurrentState.GetType().Name;
    [ShowNativeProperty] public string PreviousStateName => PreviousState.GetType().Name;

    // STATES ========================================
    public PlayerMoveState STATE_MOVE { get; private set; }
    public PlayerHideState STATE_HIDE { get; private set; }
    public PlayerHypnotizeState STATE_HYPNOTIZE { get; private set; }
    public PlayerDieState STATE_DIE { get; private set; }

    // COMPONENTS ====================================
    [Header("Reference")]
    [SerializeField] private PlayerStats stats;
    [SerializeField] private CharacterController cc;
    [SerializeField] private Collider capsuleCollider;
    [SerializeField] private CatArmy catArmy;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private ParticleSystem sprintVFX;

    // Getters
    public PlayerStats Stats { get => stats; }
    public CharacterController CC { get => cc; }
    public Collider CapsuleCollider { get => capsuleCollider; }
    public CatArmy Army { get => catArmy; }
    public Animator Animator { get => animator; }
    public SpriteRenderer PlayerRenderer { get => playerRenderer; }
    public ParticleSystem SprintVFX { get => sprintVFX; }
    public CinemachineVirtualCamera VCam;
    public Transform ClosestHideSpot { get; private set; }

    public bool IsHiding => CurrentState == STATE_HIDE;
    public bool IsDead => CurrentState == STATE_DIE;

    private void OnEnable() {
        STATE_MOVE = new PlayerMoveState(this);
        STATE_HIDE = new PlayerHideState(this);
        STATE_HYPNOTIZE = new PlayerHypnotizeState(this);
        STATE_DIE = new PlayerDieState(this);
    }

    private void Update() {
        CurrentState?.UpdateState();

        Animator.SetFloat("Speed", cc.velocity.magnitude);
        Animator.SetFloat("SignZ", cc.velocity.z);
    }

    public void ChangeState(PlayerBaseState newState) {
        if (CurrentState == newState) return;

        PreviousState = CurrentState;

        CurrentState?.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }

    private void OnTriggerEnter(Collider other) {
        if (IsHiding || IsDead) return;

        if (other.CompareTag("Dog")) {
            ChangeState(STATE_DIE);
        } else if (other.CompareTag("Cat")) {
            CatStateMachine cat = other.GetComponent<CatStateMachine>();
            Army.RegisterCat(cat, transform);

            GameplayUI.Instance.UpdateCatCount(Army.GetCatCount());
        } else if (other.CompareTag("Hide")) {
            GameplayUI.Instance.HideTextAppear("(E) Hide");
            ClosestHideSpot = other.transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Hide")) {
            GameplayUI.Instance.HideTextDissapear();
            ClosestHideSpot = null;
        }
    }
}