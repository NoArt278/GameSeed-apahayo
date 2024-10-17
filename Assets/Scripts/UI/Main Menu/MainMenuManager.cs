using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    public enum CanvasType {
        Main,
        Settings,
        Credits
    }
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Canvas _settingsCanvas;
    [SerializeField] private Canvas _creditsCanvas;

    private void Start() {
        SwitchCanvas(CanvasType.Main);
        AudioManager.Instance.Play("MenuBGM");
    }

    public void OpenMain() { SwitchCanvas(CanvasType.Main); }
    public void OpenSettings() { SwitchCanvas(CanvasType.Settings); }
    public void OpenCredits() { SwitchCanvas(CanvasType.Credits); }

    public void SwitchCanvas(CanvasType canvasType) {
        _mainCanvas.gameObject.SetActive(false);
        _settingsCanvas.gameObject.SetActive(false);
        _creditsCanvas.gameObject.SetActive(false);

        switch (canvasType) {
            case CanvasType.Main:
                _mainCanvas.gameObject.SetActive(true);
                break;
            case CanvasType.Settings:
                _settingsCanvas.gameObject.SetActive(true);
                break;
            case CanvasType.Credits:
                _creditsCanvas.gameObject.SetActive(true);
                break;
        }
    }

    public void StartGame()
    {
        SceneLoader.Instance.LoadScene("Lore");
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}