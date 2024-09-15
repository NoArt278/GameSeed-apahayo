using DG.Tweening;
using UnityEngine;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] GameObject settings;

    private void Start() {
        AudioManager.Instance.Play("MenuBGM");
    }

    public void StartGame()
    {
        SceneLoader.Instance.LoadScene("Lore");
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void ShowSetting()
    {
        settings.SetActive(true);
    }

    // Pointer enter suka miss, tp exit gk jadi ngecil mulu
    // Need fix later, for now like this
    public void CursorHover(Transform obj)
    {
        obj.DOScale(new Vector3(1.43f, 1.43f, 1.43f), 0.1f).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Hover");
    }

    public void CursorExit(Transform obj)
    {
        obj.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.1f).SetUpdate(true);
    }
}
