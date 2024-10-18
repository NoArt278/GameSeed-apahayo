using UnityEngine;

public class InputContainer : MonoBehaviour
{
    public static PlayerInput PlayerInputs;
    public static InputContainer Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            PlayerInputs = new PlayerInput();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        if (Instance == this)
            PlayerInputs.Enable();
    }

    private void OnDisable()
    {
        if (Instance == this)
            PlayerInputs.Disable();
    }
}
