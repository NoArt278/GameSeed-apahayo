using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerStats stats;
    Rigidbody rb;
    private int catAmmoCount;
    private float fireTimer;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
        fireTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = InputContainer.playerInputs.Player.Move.ReadValue<Vector2>();
        rb.velocity = stats.movementSpeed * new Vector3(moveInput.x, 0, moveInput.y);
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cat"))
        {
            catAmmoCount++;
            other.enabled = false;
        }
    }
}
