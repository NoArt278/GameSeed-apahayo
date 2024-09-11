using Cinemachine;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private CharacterController cc;
    private CapsuleCollider capsuleCollider;
    private const float maxStamina = 100, staminaDrainRate = 50, staminaFillRate = 10, staminalFillDelay = 5;
    private bool isSprinting = false, canHide = false, isHiding = false, justHid = false, canMove = false, canFillStamina = true, isDead = false;
    private float stamina, moveSpeed, lastStaminaDepleteTime;
    private Transform currTrashBin, prevTrashBin;
    private CatArmy catArmy;
    private Animator catGodAnimator;
    private SpriteRenderer sr;
    private ParticleSystem sprintParticle, sprintTrail;
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        cc = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        catArmy = GetComponent<CatArmy>();
        sprintParticle = GetComponent<ParticleSystem>();
        sprintTrail = GetComponentInChildren<ParticleSystem>();
        sr = GetComponentInChildren<SpriteRenderer>();
        catGodAnimator = GetComponentInChildren<Animator>();
        stamina = maxStamina;
        moveSpeed = stats.walkSpeed;
    }

    private void Start()
    {
        InputContainer.playerInputs.Player.Sprint.performed += StartSprint;
        InputContainer.playerInputs.Player.Sprint.canceled += StopSprintInput;
        InputContainer.playerInputs.Player.Interact.performed += Hide;
    }

    private void OnDisable()
    {
        InputContainer.playerInputs.Player.Sprint.performed -= StartSprint;
        InputContainer.playerInputs.Player.Sprint.canceled -= StopSprintInput;
        InputContainer.playerInputs.Player.Interact.performed -= Hide;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cc.isGrounded)
        {
            cc.Move(new Vector3(0, moveSpeed * Time.deltaTime * -1, 0));
        }

        if (GameManager.Instance && GameManager.Instance.CurrentState != GameState.InGame) return;

        if (canMove)
        {
            Vector2 moveInput = InputContainer.playerInputs.Player.Move.ReadValue<Vector2>();
            if (moveInput != Vector2.zero)
            {
                catGodAnimator.SetBool("isWalking", true);

                if (moveInput.x > 0) sr.flipX = true && !isSprinting;
                else if (moveInput.x < 0) sr.flipX = false || isSprinting;

                if (moveInput.y > 0) catGodAnimator.SetBool("isMovingUp", true);
                else catGodAnimator.SetBool("isMovingUp", false);
            } else
            {
                catGodAnimator.SetBool("isWalking", false);
            }
            cc.Move(new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.deltaTime);
        } else
        {
            catGodAnimator.SetBool("isWalking", false);
            StopSprint();
        }

        if (isSprinting)
        {
            stamina = Mathf.Max(stamina - Time.deltaTime * staminaDrainRate, 0);
            if (stamina == 0)
            {
                canFillStamina = false;
                lastStaminaDepleteTime = Time.time;
                GameplayUI.Instance.StaminaDeplete();
                StopSprint();
            }
        }
        else
        {
            if (canFillStamina)
            {
                stamina = Mathf.Min(stamina + Time.deltaTime * staminaFillRate, maxStamina);
            }
            else
            {
                if (Time.time - lastStaminaDepleteTime > staminalFillDelay)
                {
                    canFillStamina = true;
                }
            }
        }

        GameplayUI.Instance.UpdateStamina(stamina / maxStamina);
    }

    private void StartSprint(InputAction.CallbackContext ctx)
    {
         if (stamina > 0 && canMove)
        {
            isSprinting = true;
            moveSpeed = stats.sprintSpeed;
            catGodAnimator.SetBool("isSprinting", true);
            sprintTrail.Play();
            catArmy.StartSprint();
        }
    }

    private void StopSprintInput(InputAction.CallbackContext ctx)
    {
        StopSprint();
    }

    private void StopSprint()
    {
        isSprinting = false;
        moveSpeed = stats.walkSpeed;
        catGodAnimator.SetBool("isSprinting", false);
        catArmy.StopSprint();
        sprintTrail.Stop();
    }

    public void EnableMove()
    {
        canMove = true;
    }

    public void DisableMove()
    {
        canMove = false;
    }

    public bool IsHiding()
    {
        return isHiding;
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void Hide(InputAction.CallbackContext ctx)
    {
        if (canHide && !isHiding && !justHid)
        {
            StopAllCoroutines();
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 2;
            isHiding = true;
            canMove = false;
            cc.enabled = false;
            transform.position = currTrashBin.position + currTrashBin.forward;
            cc.enabled = true;
            StartCoroutine(HideDelay());
        } else if (isHiding && !justHid)
        {
            StopAllCoroutines();
            transform.position = currTrashBin.position + currTrashBin.forward;
            sr.enabled = true;
            isHiding = false;
            catArmy.QuitHiding(currTrashBin.GetComponent<TrashBin>().GroundPoint.position + currTrashBin.forward * 1.5f);
            currTrashBin.GetComponentInChildren<Animator>().SetBool("isHiding", false);
            StartCoroutine(HideDelay());
        }
    }

    private IEnumerator HideDelay()
    {
        justHid = true;
        GameplayUI.Instance.ChangeHideText("");
        yield return new WaitForSeconds(0.5f);
        if (isHiding)
        {
            capsuleCollider.enabled = false;
            sr.enabled = false;
            GameplayUI.Instance.ChangeHideText("(E) Unhide");
            catArmy.HideCats(currTrashBin.position);
            currTrashBin.GetComponentInChildren<Animator>().SetBool("isHiding", true);
            Transform spriteTransform = currTrashBin.GetComponentInChildren<SpriteRenderer>().transform;
            spriteTransform.LookAt(new Vector3(spriteTransform.position.x, spriteTransform.position.y, spriteTransform.position.z + 5));
            if (Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == 90 || Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == -90
                    || Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == 270)
            {
                spriteTransform.localScale = new Vector3(spriteTransform.localScale.z, spriteTransform.localScale.y, spriteTransform.localScale.x);
            }
        } else
        {
            canMove = true;
            capsuleCollider.enabled = true;
            GameplayUI.Instance.ChangeHideText("(E) Hide");
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 0;
            Transform spriteTransform = prevTrashBin.GetComponentInChildren<SpriteRenderer>().transform;
            if (Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == 90 || Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == -90
                    || Mathf.RoundToInt(spriteTransform.localRotation.eulerAngles.y) == 270)
            {
                spriteTransform.localScale = new Vector3(spriteTransform.localScale.z, spriteTransform.localScale.y, spriteTransform.localScale.x);
            }
        }
        justHid = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cat"))
        {
            CatStateMachine cat = other.gameObject.GetComponent<CatStateMachine>();
            catArmy.RegisterCat(cat, transform);
            GameplayUI.Instance.UpdateCatCount(catArmy.GetCatCount());
        } else if (other.CompareTag("Hide"))
        {
            currTrashBin = other.transform;
            prevTrashBin = currTrashBin;
            canHide = true;
            GameplayUI.Instance.HideTextAppear("(E) Hide");
        } else if (other.CompareTag("Dog") && !isHiding)
        {
            canMove = false;
            catGodAnimator.SetTrigger("Die");
            isDead = true;
            StopAllCoroutines();
            StartCoroutine(ShowGameOver());
        }
    }

    private IEnumerator ShowGameOver()
    {
        yield return new WaitForSeconds(1);
        EndGameScreen.Instance.ShowEndGameScreen();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hide"))
        {
            currTrashBin = null;
            canHide = false;
            GameplayUI.Instance.HideTextDissapear();
        }
    }
}
