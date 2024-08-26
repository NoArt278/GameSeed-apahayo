using UnityEngine;

[CreateAssetMenu(fileName = "Speed Upgrade", menuName = "Upgrades/Speed")]
public class SpeedUpgrade : PerkUpgrade {
    public float movementSpeedIncreasePercent;

    public override void ApplyUpgrade(PlayerStats playerStats) {
        playerStats.movementSpeed += playerStats.movementSpeed * movementSpeedIncreasePercent / 100;
        Debug.Log("Speed upgraded by " + movementSpeedIncreasePercent + "%");
    }
}