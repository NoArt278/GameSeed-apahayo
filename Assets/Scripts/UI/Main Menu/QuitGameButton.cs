using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneLoader.Instance.QuitGame();
    }
}
