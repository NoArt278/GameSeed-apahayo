using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerStats stats;


    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = InputContainer.playerInputs.Player.Move.ReadValue<Vector2>();
        transform.position += stats.movementSpeed * Time.deltaTime * new Vector3(moveInput.x, 0, moveInput.y);
    }
}
