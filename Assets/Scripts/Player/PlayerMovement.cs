using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private MeshRenderer mr;
    private const float maxStamina = 100, staminaDrainRate = 50, staminaFillRate = 10, minSprintStamina = 30;
    private bool isSprinting = false, canHide = false, isHiding = false, justHid = false, canMove = true;
    private float stamina, moveSpeed;
    private Transform currTrashBin;
    private CatArmy catArmy;
    private ParticleSystem sprintParticle;
    private TrailRenderer sprintTrail;
    public TMP_Text staminaText, catCountText, hideText;
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        catArmy = GetComponent<CatArmy>();
        sprintParticle = GetComponent<ParticleSystem>();
        sprintTrail = GetComponentInChildren<TrailRenderer>();
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
        if (canMove)
        {
            Vector2 moveInput = InputContainer.playerInputs.Player.Move.ReadValue<Vector2>();
            rb.velocity = moveSpeed * new Vector3(moveInput.x, 0, moveInput.y);
        } else
        {
            rb.velocity = Vector3.zero;
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
            sprintParticle.Play();
            sprintTrail.emitting = true;
        }
        catArmy.StartSprint(stats.sprintSpeed);
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
        sprintTrail.emitting = false;
    }

    private void Hide(InputAction.CallbackContext ctx)
    {
        if (canHide && !isHiding && !justHid)
        {
            StopAllCoroutines();
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 2;
            transform.position = new Vector3(currTrashBin.position.x, transform.position.y, currTrashBin.position.z) + currTrashBin.forward;
            isHiding = true;
            canMove = false;
            catArmy.HideCats(transform.position);
            StartCoroutine(HideDelay());
        } else if (isHiding && !justHid)
        {
            StopAllCoroutines();
            transform.position = new Vector3(currTrashBin.position.x, transform.position.y, currTrashBin.position.z) + currTrashBin.forward * 2;
            isHiding = false;
            mr.enabled = true;
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
            mr.enabled = false;
            hideText.text = "Press E to unhide";
        } else
        {
            canMove = true;
            capsuleCollider.enabled = true;
            hideText.text = "Press E to hide";
            catArmy.QuitHiding();
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
