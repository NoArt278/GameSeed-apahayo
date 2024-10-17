using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : SingletonMB<GameplayUI> {
    [Header("Parent Objects")]
    [SerializeField] private RectTransform hypnoBarParent;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI catCountText;
    [SerializeField] private TextMeshProUGUI hideText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI mainHintText;

    [Header("Bars")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider hypnotizeBar;

    [Header("Images")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Image overlay;
    [SerializeField] private Image hypnoVignette;
    [SerializeField] private Image dogVignette;
    [SerializeField] private Image dimmer;

    [Header("CG")]
    [SerializeField] private CanvasGroup staminaSliderCG;
    [SerializeField] private CanvasGroup pregameCG;

    [Header("Animations")]
    [SerializeField] private Animator crosshairAnimator;

    private Tween mainHintTween;

    protected override void Awake() {
        base.Awake();

        Cursor.visible = false;
        overlay.gameObject.SetActive(false);
        Color oColor = overlay.color;
        oColor.a = 1;
        overlay.color = oColor;

        mainHintText.gameObject.SetActive(false);
    }

    public void GameTransitionIn(Action onComplete = null) {
        overlay.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();

        Color dimmerColor = dimmer.color;
        dimmer.color = new Color(dimmerColor.r, dimmerColor.g, dimmerColor.b, 0.9f);

        overlay.DOFade(0, 0);
        sequence.AppendInterval(2);
        sequence.Append(pregameCG.DOFade(0, 1).OnComplete(() => {
            pregameCG.gameObject.SetActive(false);
        }));
        sequence.Join(dimmer.DOFade(0, 1).OnComplete(() => {
            dimmer.gameObject.SetActive(false);
            dimmer.color = dimmerColor;
        }));

        sequence.JoinCallback(() => {
            onComplete?.Invoke();
        }).SetDelay(0.7f);
    }

    private void Update() {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
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

        hypnoVignette.DOKill();
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

    public void StartDogChase() {
        dogVignette.gameObject.SetActive(true);
        dogVignette.color = new Color(1, 1, 1, 0);

        dogVignette.DOKill();
        dogVignette.DOFade(1f, 1.5f).SetEase(Ease.Linear).OnComplete(() => {
            DOTween.Sequence()
                .Append(dogVignette.DOFade(0.7f, 0.45f).SetEase(Ease.Linear))
                .Append(dogVignette.DOFade(1f, 0.45f).SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart);
        });
    }

    public void StopDogChase() {
        dogVignette.DOKill();
        dogVignette.DOFade(0, dogVignette.color.a * 1.5f).OnComplete(() => dogVignette.gameObject.SetActive(false));
    }

    public int GetScore()
    {
        return Convert.ToInt32(scoreText.text);
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

    public void ShowMainHintText(string text) {
        mainHintTween?.Kill();
        mainHintText.gameObject.SetActive(true);
        mainHintText.text = text;

        Vector3 initPos = new Vector3(0, 70, 0);
        Vector3 targetPos = new Vector3(0, 120, 0);

        Sequence seq = DOTween.Sequence();
        mainHintText.rectTransform.anchoredPosition = initPos;
        seq.Append(mainHintText.rectTransform.DOAnchorPosY(targetPos.y, 0.5f).SetEase(Ease.OutSine));
        seq.Join(mainHintText.DOFade(1, 0.5f));
        seq.AppendInterval(2);
        seq.Append(mainHintText.rectTransform.DOAnchorPosY(initPos.y, 0.5f).SetEase(Ease.InSine));
        seq.Join(mainHintText.DOFade(0, 0.5f));
        seq.AppendCallback(() => {
            mainHintTween = null;
            mainHintText.gameObject.SetActive(false);
            mainHintText.rectTransform.anchoredPosition = targetPos;
        });

        mainHintTween = seq;
    }
}