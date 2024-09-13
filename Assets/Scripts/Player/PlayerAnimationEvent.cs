using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour {
    // ALL METHODS IN THIS CLASS ARE CALLED FROM ANIMATION EVENTS
    [SerializeField] private PlayerHypnotize playerHypnotize;
    [SerializeField] private PlayerLaser playerLaser;

    public void OnLaserStart() {
        playerLaser.InitiateCrystal();
        playerLaser.gameObject.SetActive(true);
        playerLaser.EmitLaser(playerHypnotize.StaffPosition, playerHypnotize.HypnotizedNPCTr);
    }
}