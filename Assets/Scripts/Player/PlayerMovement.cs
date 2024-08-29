using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerStats stats;
    Rigidbody rb;
    private int catAmmoCount;
    private const float maxStamina = 100, staminaDrainRate = 50, staminaFillRate = 10, minSprintStamina = 30;
    private bool isSprinting = false;
    private float fireTimer, stamina, moveSpeed;
    [SerializeField] private TMP_Text staminaText, catCountText;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
        fireTimer = 0;
        stamina = maxStamina;
        moveSpeed = stats.walkSpeed;
    }

    private void Start()
    {
        InputContainer.playerInputs.Player.Sprint.performed += StartSprint;
        InputContainer.playerInputs.Player.Sprint.canceled += StopSprint;
    }

    private void OnDisable()
    {
        InputContainer.playerInputs.Player.Sprint.performed -= StartSprint;
        InputContainer.playerInputs.Player.Sprint.canceled -= StopSprint;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = InputContainer.playerInputs.Player.Move.ReadValue<Vector2>();
        rb.velocity = moveSpeed * new Vector3(moveInput.x, 0, moveInput.y);
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }

        if (isSprinting)
        {
            stamina = Mathf.Max(stamina - Time.deltaTime * staminaDrainRate, 0);
            if (stamina == 0)
            {
                isSprinting = false;
                moveSpeed = stats.walkSpeed;
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
         if (stamina > minSprintStamina)
        {
            isSprinting = true;
            moveSpeed = stats.sprintSpeed;
        }
    }

    private void StopSprint(InputAction.CallbackContext ctx)
    {
        isSprinting = false;
        moveSpeed = stats.walkSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cat"))
        {
            catAmmoCount++;
            catCountText.text = "Cat : " + catAmmoCount.ToString();
        }
    }
}
