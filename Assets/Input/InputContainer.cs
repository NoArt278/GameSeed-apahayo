using UnityEngine;

public class InputContainer : MonoBehaviour
{
    public static PlayerInput playerInputs;
    public static InputContainer instance;

    private void Awake()
    {
        if (instance == null)
        {
            playerInputs = new PlayerInput();
            instance = this;
            DontDestroyOnLoad(gameObject);
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
