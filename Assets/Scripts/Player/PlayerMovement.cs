using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private CharacterController cc;
    private CapsuleCollider capsuleCollider;
    private const float maxStamina = 100, staminaDrainRate = 50, staminaFillRate = 10, minSprintStamina = 0;
    private bool isSprinting = false, canHide = false, isHiding = false, justHid = false, canMove = true;
    private float stamina, moveSpeed;
    private Transform currTrashBin;
    private CatArmy catArmy;
    private Animator catGodAnimator;
    private SpriteRenderer sr;
    private ParticleSystem sprintParticle, sprintTrail;
    public TMP_Text staminaText, catCountText, hideText;
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

        if (canMove)
        {
            Vector2 moveInput = InputContainer.playerInputs.Player.Move.ReadValue<Vector2>();
            if (moveInput != Vector2.zero)
            {
                catGodAnimator.SetBool("isWalking", true);

                if (moveInput.x > 0) sr.flipX = true;
                else if (moveInput.x < 0) sr.flipX = false;

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
                StopSprint();
            }
        }
        else
        {
            stamina = Mathf.Min(stamina + Time.deltaTime * staminaFillRate, maxStamina);
        }
        staminaText.text = "Stamina : " + Mathf.RoundToInt(stamina).ToString();
    }

    private void StartSprint(InputAction.CallbackContext ctx)
    {
         if (stamina > minSprintStamina && canMove)
        {
            isSprinting = true;
            moveSpeed = stats.sprintSpeed;
            // sprintParticle.Play();
            sprintTrail.Play();
            catArmy.StartSprint(stats.sprintSpeed);
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
        catArmy.StopSprint();
        sprintTrail.Stop();
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
            isHiding = false;
            sr.enabled = true;
            catArmy.QuitHiding(currTrashBin.position + currTrashBin.forward);
            StartCoroutine(HideDelay());
        }
    }

    private IEnumerator HideDelay()
    {
        justHid = true;
        hideText.text = "";
        yield return new WaitForSeconds(0.5f);
        if (isHiding)
        {
            capsuleCollider.enabled = false;
            sr.enabled = false;
            hideText.text = "Press E to unhide";
            catArmy.HideCats(currTrashBin.position);
        } else
        {
            canMove = true;
            capsuleCollider.enabled = true;
            hideText.text = "Press E to hide";
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 0;
        }
        justHid = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cat"))
        {
            ArmyCatBehaviour armyCat = other.gameObject.GetComponent<ArmyCatBehaviour>();
            catArmy.RegisterCat(armyCat, transform);
            catCountText.text = "Cats : " + catArmy.GetCatCount().ToString();
        } else if (other.CompareTag("Hide"))
        {
            currTrashBin = other.transform;
            canHide = true;
            hideText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hide"))
        {
            currTrashBin = null;
            canHide = false;
            hideText.gameObject.SetActive(false);
        }
    }
}
