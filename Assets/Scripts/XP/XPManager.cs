using System;
using NaughtyAttributes;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    [Header("Information")]
    [ReadOnly] [SerializeField] private int currentXP;
    [ReadOnly] [SerializeField] private int currentLevel = 0;

    [Header("Progression")]
    [SerializeField] private int baseXPToLevelUp = 100;

    [Tooltip("The amount of XP required to level up will increase by this amount each time the player levels up.")]
    [SerializeField] private int xpIncreasePerLevel = 50;
    private int xpToLevelUp;

    private PerksManager perksManager;
    private UpgradesUI upgradesUI;

    // Getter
    public int CurrentXP => currentXP;
    public int CurrentLevel => currentLevel;
    public int XPToLevelUp => xpToLevelUp;

    private void Awake()
    {
        perksManager = GetComponent<PerksManager>();

        upgradesUI = GetComponent<UpgradesUI>();
        upgradesUI.Initialize(this);

        xpToLevelUp = baseXPToLevelUp;
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToLevelUp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        // Calculate for next level
        currentLevel++;
        currentXP -= xpToLevelUp;
        xpToLevelUp += xpIncreasePerLevel;

        // Pick 3 random upgrades
        PerkUpgrade[] randomPerks = perksManager.PickThreeRandomUpgrade();
        upgradesUI.Display(randomPerks);
    }

    public void ApplyUpgrade(PerkUpgrade perkUpgrade)
    {
        perksManager.ApplyUpgrade(perkUpgrade);
    }
}