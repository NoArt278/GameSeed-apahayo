using UnityEngine;

public class UpgradesUI : MonoBehaviour {
    [SerializeField] private RectTransform upgradesUI;
    [SerializeField] private UpgradeCardUI[] upgradeCardUis;

    private XPManager xpManager;
    private PerkUpgrade[] currentRandomPerks;

    public void Initialize(XPManager xpManager) {
        this.xpManager = xpManager;
    }

    public void Display(PerkUpgrade[] randomPerks) {
        currentRandomPerks = randomPerks;

        for (int i = 0; i < randomPerks.Length; i++) {
            upgradeCardUis[i].Initialize(randomPerks[i]);
        }

        upgradesUI.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SelectUpgrade(int index) {
        xpManager.ApplyUpgrade(currentRandomPerks[index]);
        upgradesUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}