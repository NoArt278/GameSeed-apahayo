using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour {
    // ALL METHODS IN THIS CLASS ARE CALLED FROM ANIMATION EVENTS
    [SerializeField] private PlayerHypnotize playerHypnotize;
    [SerializeField] private PlayerLaser playerLaser;

    public void OnCrystalStart() {
        playerLaser.InitiateCrystal();
    }

    public void OnLaserStart() {
        playerLaser.gameObject.SetActive(true);
        playerLaser.EmitLaser(playerHypnotize.StaffPosition, playerHypnotize.HypnotizedNPCTr);
    }
}