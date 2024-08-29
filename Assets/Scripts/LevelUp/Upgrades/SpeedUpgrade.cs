using UnityEngine;

[CreateAssetMenu(fileName = "Speed Upgrade", menuName = "Upgrades/Speed")]
public class SpeedUpgrade : PerkUpgrade {
    public float movementSpeedIncreasePercent;

    public override void ApplyUpgrade(PlayerStats playerStats) {
        playerStats.walkSpeed += playerStats.walkSpeed * movementSpeedIncreasePercent / 100;
        Debug.Log("Speed upgraded by " + movementSpeedIncreasePercent + "%");
    }
}