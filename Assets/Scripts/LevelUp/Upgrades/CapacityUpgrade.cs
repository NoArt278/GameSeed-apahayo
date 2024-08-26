using UnityEngine;

[CreateAssetMenu(fileName = "Capacity Upgrade", menuName = "Upgrades/Capacity")]
public class CapacityUpgrade : PerkUpgrade {
    [SerializeField] private int addedCapacity;

    public override void ApplyUpgrade(PlayerStats playerStats) {
        playerStats.maxArmyCapacity += addedCapacity;
        Debug.Log("Capacity upgraded by " + addedCapacity);
    }
}