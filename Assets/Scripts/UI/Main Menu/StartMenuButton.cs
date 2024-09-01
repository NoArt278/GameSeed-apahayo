using UnityEngine;

public class StartMenuButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneLoader.Instance.ToGameplay();
    }
}
