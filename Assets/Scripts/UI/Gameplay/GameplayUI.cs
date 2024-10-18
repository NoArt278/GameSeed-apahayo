using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : SingletonMB<GameplayUI> {
    [Header("Parent Objects")]
    [SerializeField] private RectTransform _hypnoBarParent;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _catCountText;
    [SerializeField] private TextMeshProUGUI _hideText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _mainHintText;

    [Header("Bars")]
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Slider _hypnotizeBar;

    [Header("Images")]
    [SerializeField] private Image _crosshair;
    [SerializeField] private Image _overlay;
    [SerializeField] private Image _hypnoVignette;
    [SerializeField] private Image _dogVignette;
    [SerializeField] private Image _dimmer;

    [Header("CG")]
    [SerializeField] private CanvasGroup _staminaSliderCG;
    [SerializeField] private CanvasGroup _pregameCG;

    [Header("Animations")]
    [SerializeField] private Animator _crosshairAnimator;

    private Tween _mainHintTween;

    protected override void Awake() {
        base.Awake();

        Cursor.visible = false;
        _overlay.gameObject.SetActive(false);
        Color oColor = _overlay.color;
        oColor.a = 1;
        _overlay.color = oColor;

        _mainHintText.gameObject.SetActive(false);
    }

    public void GameTransitionIn(Action onComplete = null) {
        _overlay.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();

        Color dimmerColor = _dimmer.color;
        _dimmer.color = new Color(dimmerColor.r, dimmerColor.g, dimmerColor.b, 0.9f);

        _overlay.DOFade(0, 0);
        sequence.AppendInterval(2);
        sequence.Append(_pregameCG.DOFade(0, 1).OnComplete(() => {
            _pregameCG.gameObject.SetActive(false);
        }));
        sequence.Join(_dimmer.DOFade(0, 1).OnComplete(() => {
            _dimmer.gameObject.SetActive(false);
            _dimmer.color = dimmerColor;
        }));

        sequence.JoinCallback(() => {
            onComplete?.Invoke();
        }).SetDelay(0.7f);
    }

    private void Update() {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        _crosshair.transform.position = Input.mousePosition;

        // Clamp crosshair position to screen bounds
        Vector2 mousePosition = Input.mousePosition;
        if (Camera.main != null)
        {
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Camera.main.pixelWidth);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Camera.main.pixelHeight);
        }

        _crosshair.transform.position = mousePosition;
    }

    public void UpdateCatCount(int catCount) {
        _catCountText.text = catCount.ToString();
    }

    public void ChangeHideText(string text) {
        _hideText.text = text;
    }

    public void HideTextAppear(string text) {
        ChangeHideText(text);
        _hideText.gameObject.SetActive(true);
    }

    public void HideTextDissapear() {
        _hideText.gameObject.SetActive(false);
    }

    public void UpdateStamina(float stamina) {
        _staminaSlider.value = stamina;
    }

    public void StartHypnotize() {
        _hypnoBarParent.gameObject.SetActive(true);
        _hypnotizeBar.value = 0;

        _hypnoVignette.gameObject.SetActive(true);
        _hypnoVignette.color = new Color(1, 1, 1, 0);

        _hypnoVignette.DOKill();
        _hypnoVignette.DOFade(1f, 2f).SetEase(Ease.Linear).OnComplete(() => {
            DOTween.Sequence()
                .Append(_hypnoVignette.DOFade(0.7f, 0.6f).SetEase(Ease.Linear))
                .Append(_hypnoVignette.DOFade(1f, 0.6f).SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart);
        });
    }

    public void UpdateHypnoBar(float t)
    {
        _hypnotizeBar.value = t;
    }

    public void StopHypnotize()
    {
        _hypnoBarParent.gameObject.SetActive(false);
        _hypnoVignette.DOKill();
        _hypnoVignette.DOFade(0, _hypnoVignette.color.a * 2).OnComplete(() => _hypnoVignette.gameObject.SetActive(false));
    }

    public void StartDogChase() {
        _dogVignette.gameObject.SetActive(true);
        _dogVignette.color = new Color(1, 1, 1, 0);

        _dogVignette.DOKill();
        _dogVignette.DOFade(1f, 1.5f).SetEase(Ease.Linear).OnComplete(() => {
            DOTween.Sequence()
                .Append(_dogVignette.DOFade(0.7f, 0.45f).SetEase(Ease.Linear))
                .Append(_dogVignette.DOFade(1f, 0.45f).SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart);
        });
    }

    public void StopDogChase() {
        _dogVignette.DOKill();
        _dogVignette.DOFade(0, _dogVignette.color.a * 1.5f).OnComplete(() => _dogVignette.gameObject.SetActive(false));
    }

    public int GetScore()
    {
        return Convert.ToInt32(_scoreText.text);
    }
    
    public void UpdateScore(int score)
    {
        _scoreText.text = score.ToString();
    }

    public void StaminaDeplete()
    {
        StopAllCoroutines();
        // StartCoroutine(FadeStaminaBar());
        Sequence blinkSq = DOTween.Sequence();
        blinkSq.Append(_staminaSliderCG.DOFade(0, 0.3f));
        blinkSq.Append(_staminaSliderCG.DOFade(1, 0.3f));
        blinkSq.SetLoops(8);
        blinkSq.Play();
    }

    public void PlayCrosshairBeat()
    {
        _crosshairAnimator.SetTrigger("Beat");
    }

    public void ShowMainHintText(string text) {
        _mainHintTween?.Kill();
        _mainHintText.gameObject.SetActive(true);
        _mainHintText.text = text;

        Vector3 initPos = new Vector3(0, 70, 0);
        Vector3 targetPos = new Vector3(0, 120, 0);

        Sequence seq = DOTween.Sequence();
        _mainHintText.rectTransform.anchoredPosition = initPos;
        seq.Append(_mainHintText.rectTransform.DOAnchorPosY(targetPos.y, 0.5f).SetEase(Ease.OutSine));
        seq.Join(_mainHintText.DOFade(1, 0.5f));
        seq.AppendInterval(2);
        seq.Append(_mainHintText.rectTransform.DOAnchorPosY(initPos.y, 0.5f).SetEase(Ease.InSine));
        seq.Join(_mainHintText.DOFade(0, 0.5f));
        seq.AppendCallback((TweenCallback)(() => {
            _mainHintTween = null;
            this._mainHintText.gameObject.SetActive(false);
            this._mainHintText.rectTransform.anchoredPosition = targetPos;
        }));

        _mainHintTween = seq;
    }
}