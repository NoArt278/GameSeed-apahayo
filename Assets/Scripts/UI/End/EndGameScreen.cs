using DG.Tweening;
using TMPro;
using UnityEngine;

public class EndGameScreen : MonoBehaviour {
    public static EndGameScreen Instance { get; private set; }
    [SerializeField] private CanvasGroup endGameScreen;
    [SerializeField] private TextMeshProUGUI pointsText;

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

    public void ShowEndGameScreen() {
        endGameScreen.alpha = 0;
        endGameScreen.gameObject.SetActive(true);
        pointsText.text = GameplayUI.Instance.GetScore().ToString();

        endGameScreen.DOFade(1, 1f).SetEase(Ease.InOutSine);
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
        obj.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f);
    }

    public void CursorExit(Transform obj)
    {
        obj.DOScale(new Vector3(1, 1, 1), 0.5f);
    }
}