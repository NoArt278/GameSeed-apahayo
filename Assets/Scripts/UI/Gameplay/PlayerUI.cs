using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    public static PlayerUI Instance;

    [SerializeField] private TextMeshProUGUI catCountText, hideText, scoreText;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image crosshair, staminaSliderImage;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        // Cursor.visible = false;
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

        if (HoverOverNPC()) {
            crosshair.color = Color.red;
        } else {
            crosshair.color = Color.white;
        }
    }

    private bool HoverOverNPC() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("NPC")))
        {
            return true;
        }
        return false;
    }

    public void UpdateCatCount(int catCount) {
        catCountText.text = $"x {catCount}";
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

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void StaminaDeplete()
    {
        StopAllCoroutines();
        StartCoroutine(FadeStaminaBar());
    }

    private IEnumerator FadeStaminaBar()
    {
        float startTime = Time.time;
        while (Time.time - startTime < 5)
        {
            staminaSliderImage.DOFade(0, 0.3f);
            yield return new WaitForSeconds(0.3f);
            staminaSliderImage.DOFade(1, 0.3f);
            yield return new WaitForSeconds(0.3f);
        }
    }
}