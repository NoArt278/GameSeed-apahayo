using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    public static PlayerUI Instance;

    [SerializeField] private TextMeshProUGUI catCountText, hideText;
    [SerializeField] private Slider staminaSlider;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
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
}