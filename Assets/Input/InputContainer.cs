using UnityEngine;

public class InputContainer : MonoBehaviour
{
    public static PlayerInput playerInputs;
    public static InputContainer instance;

    private void Awake()
    {
        playerInputs = new PlayerInput();
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        if (instance == this)
            playerInputs.Enable();
    }

    private void OnDisable()
    {
        if (instance == this)
            playerInputs.Disable();
    }
}
