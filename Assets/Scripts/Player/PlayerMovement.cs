using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private MeshRenderer mr;
    private int catAmmoCount;
    private const float maxStamina = 100, staminaDrainRate = 50, staminaFillRate = 10, minSprintStamina = 30;
    private bool isSprinting = false, canHide = false, isHiding = false, justHid = false, canMove = true;
    private float stamina, moveSpeed;
    [SerializeField] private TMP_Text staminaText, catCountText, hideText;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
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
    }

    private void Hide(InputAction.CallbackContext ctx)
    {
        if (canHide && !isHiding && !justHid)
        {
            StopAllCoroutines();
            StartCoroutine(HideDelay());
            isHiding = true;
            canMove = false;
            capsuleCollider.enabled = false;
            mr.enabled = false;
            hideText.text = "Press E to unhide";
        } else if (isHiding && !justHid)
        {
            StopAllCoroutines();
            StartCoroutine(HideDelay());
            isHiding = false;
            canMove = true;
            capsuleCollider.enabled = true;
            mr.enabled = true;
            hideText.text = "Press E to hide";
        }
    }

    private IEnumerator HideDelay()
    {
        justHid = true;
        yield return new WaitForSeconds(0.5f);
        justHid = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cat"))
        {
            catAmmoCount++;
            catCountText.text = "Cat : " + catAmmoCount.ToString();
        } else if (other.CompareTag("Hide"))
        {
            canHide = true;
            hideText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hide"))
        {
            canHide = false;
            hideText.gameObject.SetActive(false);
        }
    }
}
