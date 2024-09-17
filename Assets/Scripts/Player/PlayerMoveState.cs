using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMoveState : PlayerBaseState {
    public PlayerMoveState(PlayerStateMachine stm) : base(stm) { }
    // STATS
    private const float 
        maxStamina = 100f,
        staminaDrainRate = 50f,
        staminaRegenRate = 10f,
        staminaRegenDelay = 5f;

    private float 
        currentStamina,
        lastStaminaDepleteTime,
        moveSpeed;

    private bool isSprinting = false;

    public override void EnterState() {
        moveSpeed = stm.Stats.walkSpeed;
        currentStamina = maxStamina;

        // Register inputs
        InputContainer.playerInputs.Player.Sprint.started += PlaySprintAudio;
        InputContainer.playerInputs.Player.Sprint.performed += StartSprint;
        InputContainer.playerInputs.Player.Sprint.canceled += StopSprint;
        InputContainer.playerInputs.Player.Interact.started += Hide;
    }

    private void PlaySprintAudio(InputAction.CallbackContext _) {
        if (currentStamina > 0.5f) AudioManager.Instance.Play("Sprint");
    }

    private void StartSprint(InputAction.CallbackContext _)
    {
        if (currentStamina > 0.5f)
        {
            isSprinting = true;
            moveSpeed = stm.Stats.sprintSpeed;
            stm.Animator.SetBool("isSprinting", true);
            stm.SprintVFX.Play();
            stm.Army.StartSprint();
        } else {
            AudioManager.Instance.PlayOneShot("Cant");
            GameplayUI.Instance.ShowMainHintText("No Stamina!");
        }
    }

    private void StopSprint(InputAction.CallbackContext _)
    {
        isSprinting = false;
        moveSpeed = stm.Stats.walkSpeed;
        stm.Animator.SetBool("isSprinting", false);
        stm.Army.StopSprint();
        stm.SprintVFX.Stop();
        AudioManager.Instance.StopFadeOut("Sprint", 1f);
    }


    private void Hide(InputAction.CallbackContext _) {
        stm.ChangeState(stm.STATE_HIDE);
    }

    public override void UpdateState() {
        if (!stm.CC.isGrounded) {
            stm.CC.Move(9.81f * Time.deltaTime * Vector3.down);
        }

        if (GameManager.Instance.CurrentState != GameState.InGame) return;

        Vector2 moveInput = InputContainer.playerInputs.Player.Move.ReadValue<Vector2>();
        stm.PlayerRenderer.flipX = moveInput.x > 0;

        stm.CC.Move(moveSpeed * Time.deltaTime * new Vector3(moveInput.x, 0, moveInput.y));

        if (isSprinting) {
            currentStamina = Mathf.Max(0, currentStamina - Time.deltaTime * staminaDrainRate);
            if (currentStamina <= 0) {
                lastStaminaDepleteTime = Time.time;
                GameplayUI.Instance.StaminaDeplete(0.6f, staminaRegenDelay);
                StopSprint(default);
            }
        } else {
            if (Time.time - lastStaminaDepleteTime > staminaRegenDelay) {
                currentStamina = Mathf.Min(maxStamina, currentStamina + Time.deltaTime * staminaRegenRate);
            }
        }

        GameplayUI.Instance.UpdateStamina(currentStamina / maxStamina);
    }

    public override void ExitState() {
        // Unregister inputs
        InputContainer.playerInputs.Player.Sprint.started -= PlaySprintAudio;
        InputContainer.playerInputs.Player.Sprint.performed -= StartSprint;
        InputContainer.playerInputs.Player.Sprint.canceled -= StopSprint;
        InputContainer.playerInputs.Player.Interact.started -= Hide;
    }
}