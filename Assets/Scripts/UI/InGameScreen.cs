using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameScreen : MonoBehaviour {
    public static InGameScreen Instance { get; private set; }
    [SerializeField] private CanvasGroup endGameScreen;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private Image dimmer;
    [SerializeField] private RectTransform settingPanel;

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
        InputContainer.playerInputs.Player.Pause.started += ctx => ToggleSettingPanel();
    }

    private void OnDisable() {
        InputContainer.playerInputs.Player.Pause.started -= ctx => ToggleSettingPanel();
    }

    private void OnDestroy() {
        InputContainer.playerInputs.Player.Pause.started -= ctx => ToggleSettingPanel();
    }

    public void ShowEndGameScreen() {
        endGameScreen.alpha = 0;
        endGameScreen.gameObject.SetActive(true);
        pointsText.text = GameplayUI.Instance.GetScore().ToString();

        InputContainer.playerInputs.Player.Pause.started -= ctx => ToggleSettingPanel();
        endGameScreen.DOFade(1, 1f).SetEase(Ease.InOutSine).SetUpdate(true);

        GameManager.Instance.SetGameState(GameState.PostGame);
        Cursor.visible = true;
        Time.timeScale = 0;
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

    public void ToggleSettingPanel()
    {
        if (isSettingPanelOpen)
        {
            CloseSettingPanel();
        }
        else
        {
            OpenSettingPanel();
        }

        isSettingPanelOpen = !isSettingPanelOpen;
    }

    public void OpenSettingPanel()
    {
        dimmer.gameObject.SetActive(true);
        settingPanel.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutSine).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Click");

        GameManager.Instance.SetGameState(GameState.Paused);
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void CloseSettingPanel()
    {
        dimmer.gameObject.SetActive(false);
        Vector3 pos = new(-settingPanel.rect.width, 0, 0);

        settingPanel.DOAnchorPos(pos, 0.5f).SetEase(Ease.InSine).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Click");

        GameManager.Instance.SetGameState(GameState.InGame);
        Cursor.visible = false;
        Time.timeScale = 1;
    }
}