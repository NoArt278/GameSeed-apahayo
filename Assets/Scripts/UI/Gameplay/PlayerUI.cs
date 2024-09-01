using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour {
    public static PlayerUI Instance;

    [SerializeField] private TextMeshProUGUI catCountText, hideText, staminaText;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void Initialize(PlayerMovement player) {
        player.catCountText = catCountText;
        player.hideText = hideText;
        player.staminaText = staminaText;
    }
}