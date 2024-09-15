using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameScreen : MonoBehaviour {
    public static InGameScreen Instance { get; private set; }
    [SerializeField] private CanvasGroup endGameScreen;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI bestLabel;
    [SerializeField] private Image dimmer;
    [SerializeField] private RectTransform sidePanel;

    [Header("Setting Groups")]
    [SerializeField] private RectTransform main;
    [SerializeField] private RectTransform setting;

    private bool isSettingPanelOpen = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        endGameScreen.gameObject.SetActive(false);
    }

    public void Initialize() {
        InputContainer.playerInputs.Player.Pause.started += ToggleSettingPanel;
    }

    private void OnDisable() {
        InputContainer.playerInputs.Player.Pause.started -= ToggleSettingPanel;
    }

    public void ShowEndGameScreen() {
        endGameScreen.alpha = 0;
        endGameScreen.gameObject.SetActive(true);
        int currentScore = GameplayUI.Instance.GetScore();
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        pointsText.text = currentScore.ToString();

        if (currentScore > highScore) {
            PlayerPrefs.SetInt("HighScore", GameplayUI.Instance.GetScore());
            highScore = currentScore;
            bestLabel.text = "New Best!";
        }

        highScoreText.text = highScore.ToString();

        InputContainer.playerInputs.Player.Pause.started -= ToggleSettingPanel;
        AudioManager.Instance.StopBGMFadeOut(0.2f);
        endGameScreen.DOFade(1, 1f).SetEase(Ease.InOutSine).SetUpdate(true);
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
        if (isSettingPanelOpen)
        {
            CloseSidePanel();
        }
        else
        {
            OpenSidePanel();
        }

        isSettingPanelOpen = !isSettingPanelOpen;
    }

    public void OpenSidePanel()
    {
        dimmer.gameObject.SetActive(true);
        sidePanel.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutSine).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Click");

        GameManager.Instance.SetGameState(GameState.Paused);
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void OpenSetting() {
        main.gameObject.SetActive(false);
        setting.gameObject.SetActive(true);
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void CloseSetting() {
        main.gameObject.SetActive(true);
        setting.gameObject.SetActive(false);
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void CloseSidePanel()
    {
        dimmer.gameObject.SetActive(false);
        Vector3 pos = new(-sidePanel.rect.width, 0, 0);

        sidePanel.DOAnchorPos(pos, 0.5f).SetEase(Ease.InSine).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Click");

        GameManager.Instance.SetGameState(GameState.InGame);
        Cursor.visible = false;
        Time.timeScale = 1;
    }
}