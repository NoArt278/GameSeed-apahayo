using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour {
    public static GameplayUI Instance;

    [Header("Parent Objects")]
    [SerializeField] private RectTransform hypnoBarParent;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI catCountText;
    [SerializeField] private TextMeshProUGUI hideText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Bars")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider hypnotizeBar;

    [Header("Images")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Image overlay;
    [SerializeField] private Image hypnoVignette;

    [Header("CG")]
    [SerializeField] private CanvasGroup staminaSliderCG;

    [Header("Animations")]
    [SerializeField] private Animator crosshairAnimator;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        // Cursor.visible = false;
        overlay.gameObject.SetActive(false);
        Color oColor = overlay.color;
        oColor.a = 1;
        overlay.color = oColor;
    }

    public void GameTransitionIn(Action onComplete = null) {
        overlay.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(1);
        sequence.Append(overlay.DOFade(0, 1).OnComplete(() => {
            overlay.gameObject.SetActive(false);
        }));

        sequence.JoinCallback(() => {
            onComplete?.Invoke();
        }).SetDelay(0.7f);
    }

    private void Update() {
        crosshair.transform.position = Input.mousePosition;

        // Clamp crosshair position to screen bounds
        Vector2 mousePosition = Input.mousePosition;
        if (Camera.main != null)
        {
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Camera.main.pixelWidth);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Camera.main.pixelHeight);
        }

        crosshair.transform.position = mousePosition;
    }

    public void UpdateCatCount(int catCount) {
        catCountText.text = catCount.ToString();
    }

    public void ChangeHideText(string text) {
        hideText.text = text;
    }

    public void HideTextAppear(string text) {
        ChangeHideText(text);
        hideText.gameObject.SetActive(true);
    }

    public void HideTextDissapear() {
        hideText.gameObject.SetActive(false);
    }

    public void UpdateStamina(float stamina) {
        staminaSlider.value = stamina;
    }

    public void StartHypnotize() {
        hypnoBarParent.gameObject.SetActive(true);
        hypnotizeBar.value = 0;

        hypnoVignette.gameObject.SetActive(true);
        hypnoVignette.color = new Color(1, 1, 1, 0);

        hypnoVignette.DOFade(1f, 2f).SetEase(Ease.Linear).OnComplete(() => {
            DOTween.Sequence()
                .Append(hypnoVignette.DOFade(0.7f, 0.6f).SetEase(Ease.Linear))
                .Append(hypnoVignette.DOFade(1f, 0.6f).SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart);
        });
    }

    public void UpdateHypnoBar(float t)
    {
        hypnotizeBar.value = t;
    }

    public void StopHypnotize()
    {
        hypnoBarParent.gameObject.SetActive(false);
        hypnoVignette.DOKill();
        hypnoVignette.DOFade(0, hypnoVignette.color.a * 2).OnComplete(() => hypnoVignette.gameObject.SetActive(false));
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void StaminaDeplete()
    {
        StopAllCoroutines();
        // StartCoroutine(FadeStaminaBar());
        Sequence blinkSq = DOTween.Sequence();
        blinkSq.Append(staminaSliderCG.DOFade(0, 0.3f));
        blinkSq.Append(staminaSliderCG.DOFade(1, 0.3f));
        blinkSq.SetLoops(8);
        blinkSq.Play();
    }

    public void PlayCrosshairBeat()
    {
        crosshairAnimator.SetTrigger("Beat");
    }
}