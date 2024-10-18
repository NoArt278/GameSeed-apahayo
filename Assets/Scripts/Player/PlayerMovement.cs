using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private CharacterController cc;
    private CapsuleCollider capsuleCollider;
    private const float MAX_STAMINA = 100;
    private const float STAMINA_DRAIN_RATE = 50;
    private const float STAMINA_FILL_RATE = 10;
    private const float STAMINA_FILL_DELAY = 5;

    private bool _isSprinting = false;
    private bool _canHide = false;
    private bool _isHiding = false;
    private bool _justHid = false;
    private bool _canMove = false;
    private bool _canFillStamina = true;
    private bool _isDead = false;

    private float _stamina;
    private float _moveSpeed;
    private float _lastStaminaDepleteTime;
    private Transform _currTrashBin;
    private Transform _prevTrashBin;
    private CatArmy _catArmy;
    private Animator _catGodAnimator;
    private SpriteRenderer _sr;
    private ParticleSystem _sprintTrail;
    [HideInInspector] public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        cc = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        _catArmy = GetComponent<CatArmy>();
        _sprintTrail = GetComponentInChildren<ParticleSystem>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _catGodAnimator = GetComponentInChildren<Animator>();
        _stamina = MAX_STAMINA;
        _moveSpeed = stats.WalkSpeed;
    }

    private void Start()
    {
        InputContainer.PlayerInputs.Player.Sprint.started += PlaySprintAudio;
        InputContainer.PlayerInputs.Player.Sprint.performed += StartSprint;
        InputContainer.PlayerInputs.Player.Sprint.canceled += StopSprintInput;
        InputContainer.PlayerInputs.Player.Interact.performed += Hide;
    }

    private void OnDisable()
    {
        InputContainer.PlayerInputs.Player.Sprint.performed -= StartSprint;
        InputContainer.PlayerInputs.Player.Sprint.canceled -= StopSprintInput;
        InputContainer.PlayerInputs.Player.Interact.performed -= Hide;
    }

    private void PlaySprintAudio(InputAction.CallbackContext _) {
        if (_stamina > 0.5f && _canMove) AudioManager.Instance.Play("Sprint");
    }

    // Update is called once per frame
    void Update()
    {
        if (!cc.isGrounded)
        {
            cc.Move(new Vector3(0, _moveSpeed * Time.deltaTime * -1, 0));
        }

        if (GameManager.Instance && GameManager.Instance.CurrentState != GameState.InGame) return;

        if (_canMove)
        {
            Vector2 moveInput = InputContainer.PlayerInputs.Player.Move.ReadValue<Vector2>();
            if (moveInput != Vector2.zero)
            {
                _catGodAnimator.SetBool("isWalking", true);

                if (moveInput.x > 0) _sr.flipX = true && !_isSprinting;
                else if (moveInput.x < 0) _sr.flipX = false || _isSprinting;

                if (moveInput.y > 0) _catGodAnimator.SetBool("isMovingUp", true);
                else _catGodAnimator.SetBool("isMovingUp", false);
            } else
            {
                _catGodAnimator.SetBool("isWalking", false);
            }
            cc.Move(new Vector3(moveInput.x, 0, moveInput.y) * _moveSpeed * Time.deltaTime);
        } else
        {
            _catGodAnimator.SetBool("isWalking", false);
            StopSprint();
        }

        if (_isSprinting)
        {
            _stamina = Mathf.Max(_stamina - Time.deltaTime * STAMINA_DRAIN_RATE, 0);
            if (_stamina == 0)
            {
                _canFillStamina = false;
                _lastStaminaDepleteTime = Time.time;
                GameplayUI.Instance.StaminaDeplete();
                StopSprint();
            }
        }
        else
        {
            if (_canFillStamina)
            {
                _stamina = Mathf.Min(_stamina + Time.deltaTime * STAMINA_FILL_RATE, MAX_STAMINA);
            }
            else
            {
                if (Time.time - _lastStaminaDepleteTime > STAMINA_FILL_DELAY)
                {
                    _canFillStamina = true;
                }
            }
        }

        GameplayUI.Instance.UpdateStamina(_stamina / MAX_STAMINA);
    }

    private void StartSprint(InputAction.CallbackContext ctx)
    {
         if (_stamina > 0 && _canMove)
        {
            _isSprinting = true;
            _moveSpeed = stats.SprintSpeed;
            _catGodAnimator.SetBool("isSprinting", true);
            _sprintTrail.Play();
            _catArmy.StartSprint();
        } else {
            AudioManager.Instance.PlayOneShot("Cant");
            GameplayUI.Instance.ShowMainHintText("No Stamina!");
        }
    }

    private void StopSprintInput(InputAction.CallbackContext ctx)
    {
        StopSprint();
    }

    private void StopSprint()
    {
        _isSprinting = false;
        _moveSpeed = stats.WalkSpeed;
        _catGodAnimator.SetBool("isSprinting", false);
        _catArmy.StopSprint();
        _sprintTrail.Stop();
        AudioManager.Instance.StopFadeOut("Sprint", 1f);
    }

    public void EnableMove()
    {
        _canMove = true;
    }

    public void DisableMove()
    {
        _canMove = false;
    }

    public bool IsHiding()
    {
        return _isHiding;
    }

    public bool IsDead()
    {
        return _isDead;
    }

    private void Hide(InputAction.CallbackContext ctx)
    {
        if (_canHide && !_isHiding && !_justHid)
        {
            StopAllCoroutines();
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 2;
            _isHiding = true;
            _canMove = false;
            cc.enabled = false;
            transform.position = _currTrashBin.position + _currTrashBin.forward;
            cc.enabled = true;
            // Hide();
            StartCoroutine(HideDelay());
        } else if (_isHiding && !_justHid)
        {
            StopAllCoroutines();
            transform.position = _currTrashBin.position + _currTrashBin.forward;
            _sr.enabled = true;
            _isHiding = false;
            _catArmy.QuitHiding(_currTrashBin.GetComponent<TrashBin>().GroundPoint.position + _currTrashBin.forward * 1.5f);
            _currTrashBin.GetComponentInChildren<Animator>().SetBool("isHiding", false);
            // Hide();
            StartCoroutine(HideDelay());
        }
    }

    private IEnumerator HideDelay()
    {
        _justHid = true;
        GameplayUI.Instance.ChangeHideText("");
        if (_isHiding) {
            VFXManager.Instance.PlayPoofVFX(transform.position);
        }
        yield return new WaitForSeconds(0.3f);
        if (_isHiding)
        {
            capsuleCollider.enabled = false;
            _sr.enabled = false;
            GameplayUI.Instance.ChangeHideText("(E) Unhide");
            _catArmy.HideCats(_currTrashBin.position);
            _currTrashBin.GetComponentInChildren<Animator>().SetBool("isHiding", true);
            Transform spriteTransform = _currTrashBin.GetComponentInChildren<SpriteRenderer>().transform;
            spriteTransform.LookAt(new Vector3(spriteTransform.position.x, spriteTransform.position.y, spriteTransform.position.z + 5));
            if (Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == 90 || Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == -90
                    || Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == 270)
            {
                spriteTransform.localScale = new Vector3(spriteTransform.localScale.z, spriteTransform.localScale.y, spriteTransform.localScale.x);
            }
        } else
        {
            _canMove = true;
            capsuleCollider.enabled = true;
            GameplayUI.Instance.ChangeHideText("(E) Hide");
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 0;
            Transform spriteTransform = _prevTrashBin.GetComponentInChildren<SpriteRenderer>().transform;
            if (Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == 90 || Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == -90
                    || Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == 270)
            {
                spriteTransform.localScale = new Vector3(spriteTransform.localScale.z, spriteTransform.localScale.y, spriteTransform.localScale.x);
            }
        }
        _justHid = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cat") && !_isHiding)
        {
            CatStateMachine cat = other.gameObject.GetComponent<CatStateMachine>();
            _catArmy.RegisterCat(cat, transform);
            GameplayUI.Instance.UpdateCatCount(_catArmy.GetCatCount());
        } else if (other.CompareTag("Hide") && !_isHiding)
        {
            _currTrashBin = other.transform;
            _prevTrashBin = _currTrashBin;
            _canHide = true;
            GameplayUI.Instance.HideTextAppear("(E) Hide");
        } else if (other.CompareTag("Dog") && !_isHiding)
        {
            _canMove = false;
            _canHide = false;
            _catGodAnimator.SetTrigger("Die");
            AudioManager.Instance.PlayOneShot("Caught");
            AudioManager.Instance.PlayOneShot("Yeet");
            _isDead = true;
            StopAllCoroutines();
            StartCoroutine(ShowGameOver());
        }
    }

    private IEnumerator ShowGameOver()
    {
        yield return new WaitForSeconds(1);
        InGameScreen.Instance.ShowEndGameScreen();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hide"))
        {
            _currTrashBin = null;
            _canHide = false;
            GameplayUI.Instance.HideTextDissapear();
        }
    }
}
