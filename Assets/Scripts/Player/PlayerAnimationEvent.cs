using UnityEngine;

// ALL METHODS IN THIS CLASS ARE CALLED FROM ANIMATION EVENTS
// DO NOT CALL THEM FROM ANYWHERE ELSE
public class PlayerAnimationEvent : MonoBehaviour {
    [SerializeField] private PlayerHypnotize playerHypnotize;
    [SerializeField] private PlayerLaser playerLaser;

    public void OnLaserStart() {
        playerLaser.InitiateCrystal();
        playerLaser.gameObject.SetActive(true);
        playerLaser.EmitLaser(playerHypnotize.StaffPosition, playerHypnotize.HypnotizedNPCTr);
    }
}