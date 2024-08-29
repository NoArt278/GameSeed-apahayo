using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerStats stats;
    [SerializeField] private GameObject cat;
    private GameObject currCat;
    private float fireTimer, catLaunchSpeed = 8;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        fireTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector2 moveInput = InputContainer.playerInputs.Player.Move.ReadValue<Vector2>();
        transform.position += stats.movementSpeed * Time.deltaTime * new Vector3(moveInput.x, 0, moveInput.y);
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
    }
}
