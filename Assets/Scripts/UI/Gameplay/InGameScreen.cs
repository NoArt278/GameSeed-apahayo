using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameScreen : SingletonMB<InGameScreen> {
    [SerializeField] private CanvasGroup _endGameScreen;
    [SerializeField] private TextMeshProUGUI _pointsText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _bestLabel;
    [SerializeField] private Image _dimmer;
    [SerializeField] private RectTransform _sidePanel;

    [Header("Setting Groups")]
    [SerializeField] private RectTransform _main;
    [SerializeField] private RectTransform _setting;

    private bool _isSettingPanelOpen = false;

    private void Start() {
        _endGameScreen.gameObject.SetActive(false);
    }

    public void Initialize() {
        InputContainer.PlayerInputs.Player.Pause.started += ToggleSettingPanel;
    }

    private void OnDisable() {
        InputContainer.PlayerInputs.Player.Pause.started -= ToggleSettingPanel;
    }

    public void ShowEndGameScreen() {
        _endGameScreen.alpha = 0;
        _endGameScreen.gameObject.SetActive(true);
        int currentScore = GameplayUI.Instance.GetScore();
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        _pointsText.text = currentScore.ToString();

        if (currentScore > highScore) {
            PlayerPrefs.SetInt("HighScore", GameplayUI.Instance.GetScore());
            highScore = currentScore;
            _bestLabel.text = "New Best!";
        }

        _highScoreText.text = highScore.ToString();

        InputContainer.PlayerInputs.Player.Pause.started -= ToggleSettingPanel;
        AudioManager.Instance.Stop("Time");
        AudioManager.Instance.StopBGMFadeOut(0.2f);
        _endGameScreen.DOFade(1, 1f).SetEase(Ease.InOutSine).SetUpdate(true);
        DOVirtual.DelayedCall(0.2f, () => AudioManager.Instance.PlayOneShot("GameOver"));

        GameManager.Instance.SetGameState(GameState.PostGame);
        Cursor.visible = true;
        Time.timeScale = 0.0f;
    }

    public void RestartScene() {
        SceneLoader.Instance.RestartGameplay();
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void ToMainMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void CursorHover(Transform obj)
    {
        obj.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Hover");
    }

    public void CursorExit(Transform obj)
    {
        obj.DOScale(new Vector3(1, 1, 1), 0.1f).SetUpdate(true);
    }

    public void ToggleSettingPanel(InputAction.CallbackContext _)
    {
        ToggleSettingPanel();
    }

    public void ToggleSettingPanel()
    {
        if (_isSettingPanelOpen)
        {
            CloseSidePanel();
        }
        else
        {
            OpenSidePanel();
        }

        _isSettingPanelOpen = !_isSettingPanelOpen;
    }

    public void OpenSidePanel()
    {
        _dimmer.gameObject.SetActive(true);
        _sidePanel.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutSine).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Click");

        GameManager.Instance.SetGameState(GameState.Paused);
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void OpenSetting() {
        _main.gameObject.SetActive(false);
        _setting.gameObject.SetActive(true);
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void CloseSetting() {
        _main.gameObject.SetActive(true);
        _setting.gameObject.SetActive(false);
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void CloseSidePanel()
    {
        _dimmer.gameObject.SetActive(false);
        Vector3 pos = new(-_sidePanel.rect.width, 0, 0);

        _sidePanel.DOAnchorPos(pos, 0.5f).SetEase(Ease.InSine).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Click");

        GameManager.Instance.SetGameState(GameState.InGame);
        Cursor.visible = false;
        Time.timeScale = 1;
    }
}