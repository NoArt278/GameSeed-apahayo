using UnityEngine;

// ALL METHODS IN THIS CLASS ARE CALLED FROM ANIMATION EVENTS
// DO NOT CALL THEM FROM ANYWHERE ELSE
public class PlayerAnimationEvent : MonoBehaviour {
    [SerializeField] private PlayerHypnotize _playerHypnotize;
    [SerializeField] private PlayerLaser _playerLaser;

    public void OnLaserStart() {
        _playerLaser.InitiateCrystal();
        _playerLaser.gameObject.SetActive(true);
        _playerLaser.EmitLaser(_playerHypnotize.StaffPosition, _playerHypnotize.HypnotizedNPCTr);
    }
}