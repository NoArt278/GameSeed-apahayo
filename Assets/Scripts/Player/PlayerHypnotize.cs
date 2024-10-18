using UnityEngine;

public class PlayerHypnotize : MonoBehaviour
{
    private CatArmy _catArmy;
    private NPCStateMachine _currNPC;
    private NPCStateMachine _lastHypnotizedNPC;
    private PlayerMovement _playerMovement;
    private SpriteRenderer _sr;
    private Animator _catGodAnimator;
    private float _lastClickTime;
    private const float CLICK_MOVE_DELAY = 0.5f;
    private int _score = 0;
    private bool _delayPassed = false;

    [SerializeField] private PlayerLaser _playerLaser;
    [SerializeField] private Transform _catFloatCenter;
    [SerializeField] private Transform _staffPosition;
    [SerializeField] private float _distance = 0.8f;

    private Vector3 _originalStaffPosition;

    public Transform StaffPosition { get {
        Vector3 stPos = _originalStaffPosition;
        stPos.x *= _sr.flipX ? -1 : 1;

        _staffPosition.localPosition = stPos;
        return _staffPosition;
    }}
    
    public Transform HypnotizedNPCTr { get; private set; }

    private void Awake()
    {
        _catArmy = GetComponent<CatArmy>();
        _playerMovement = GetComponent<PlayerMovement>();
        _catGodAnimator = GetComponentInChildren<Animator>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _lastClickTime = 0;

        _originalStaffPosition = _staffPosition.localPosition;
    }

    private void Start() {
        GameplayUI.Instance.UpdateScore(_score);
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        if (_playerMovement.IsHiding() || _playerMovement.IsDead())
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.SphereCast(ray.origin, 1, ray.direction, out hit, Mathf.Infinity, LayerMask.GetMask("NPC")))
        {
            _currNPC = hit.collider.GetComponent<NPCStateMachine>();
            if (InputContainer.PlayerInputs.Player.Fire.WasPerformedThisFrame())
            {
                _sr.flipX = _currNPC.transform.position.x < transform.position.x;
                if (_catArmy.GetCatCount() > 0 && !_currNPC.CheckCrazed())
                {
                    if (_lastHypnotizedNPC == null)
                    {
                        _lastHypnotizedNPC = _currNPC;
                        _lastHypnotizedNPC.IsControllingBar = true;
                        HypnotizedNPCTr = _currNPC.transform;
                        _playerLaser.EmitLaser(StaffPosition, HypnotizedNPCTr);
                        GameplayUI.Instance.StartHypnotize();
                        Vector3 direction = (_currNPC.Center.position - transform.position).normalized;
                        _catArmy.StartHypnotize(_catFloatCenter.position + direction * _distance);
                        _currNPC.StartHyponotize();
                        if (!_catGodAnimator.GetBool("isHypnotizing"))
                        {
                            _catGodAnimator.SetTrigger("StartHypnotize");
                            _catGodAnimator.SetBool("isHypnotizing", true);
                        }
                    }
                }
                else if (_catArmy.GetCatCount() <= 0 && !_currNPC.CheckCrazed())
                {
                    _currNPC.Animator.SetBool("isHypno", false);
                    if(_currNPC.CurrentState != _currNPC.STATE_RANDOMMOVE){
                        _currNPC.TransitionToState(_currNPC.STATE_RANDOMMOVE);                         
                    }

                    AudioManager.Instance.PlayOneShot("Cant");
                    GameplayUI.Instance.ShowMainHintText("You have no cat!");
                }
                if (!_currNPC.CheckCrazed() && _currNPC == _lastHypnotizedNPC && _catArmy.GetCatCount() > 0)
                {
                    _playerMovement.DisableMove();
                    if (!_catGodAnimator.GetBool("isHypnotizing"))
                    {
                        _catGodAnimator.SetTrigger("StartHypnotize");
                        _catGodAnimator.SetBool("isHypnotizing", true);
                    }
                    _lastClickTime = Time.time;
                    AudioManager.Instance.PlayOneShot("ClickHypno");
                    GameplayUI.Instance.PlayCrosshairBeat();
                    if (_delayPassed) {
                        Vector3 direction = (_currNPC.Center.position - transform.position).normalized;
                        _catArmy.CancelHypnotize();
                        _catArmy.StartHypnotize(_catFloatCenter.position + direction * _distance);
                        _delayPassed = false;
                    }
                    _currNPC.OnNPCClicked();
                }
            }
        } else
        {
            _currNPC = null;
        }

        if (Time.time - _lastClickTime > CLICK_MOVE_DELAY && !_playerMovement.IsHiding())
        {
            _catGodAnimator.SetBool("isHypnotizing", false);
            _playerMovement.EnableMove();
            if (_lastHypnotizedNPC != null)
            {
                _playerLaser.StopLaser();
                _catArmy.CancelHypnotize();
                _delayPassed = true;
            }
        }
        else if (_playerMovement.IsHiding())
        {
            _lastClickTime = Time.time;
        }

        if (_lastHypnotizedNPC != null)
        {
            if (_lastHypnotizedNPC.CheckCrazed())
            {
                _score += Mathf.RoundToInt(_lastHypnotizedNPC.HypnotizeStats.hypnotizeHealth * 10);
                _catArmy.DestroyCat(_lastHypnotizedNPC.transform.position + Vector3.up * 0.5f);
                GameplayUI.Instance.UpdateCatCount(_catArmy.GetCatCount());
                GameplayUI.Instance.UpdateScore(_score);
                GameplayUI.Instance.StopHypnotize();
                AudioManager.Instance.PlayOneShot("HypnoSuccess");
                _playerLaser.StopLaser();
                _lastHypnotizedNPC.IsControllingBar = false;
                _lastHypnotizedNPC = null;
            }
            else if (!_lastHypnotizedNPC.CheckHypnotize())
            {
                GameplayUI.Instance.StopHypnotize();
                _catArmy.CancelHypnotize();
                _playerLaser.StopLaser();
                _lastHypnotizedNPC.IsControllingBar = false;
                _lastHypnotizedNPC = null;
            }
        }
    }
}
