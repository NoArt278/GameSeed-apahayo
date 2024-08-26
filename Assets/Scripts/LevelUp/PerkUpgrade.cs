using UnityEngine;

public abstract class PerkUpgrade : ScriptableObject {
    public string upgradeName;
    public string upgradeDescription;

    public abstract void ApplyUpgrade(PlayerStats playerStats);
}