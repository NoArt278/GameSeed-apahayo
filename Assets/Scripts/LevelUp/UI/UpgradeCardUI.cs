using TMPro;
using UnityEngine;

public class UpgradeCardUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Initialize(PerkUpgrade perkUpgrade) {
        nameText.text = perkUpgrade.upgradeName;
        descriptionText.text = perkUpgrade.upgradeDescription;
    }
}