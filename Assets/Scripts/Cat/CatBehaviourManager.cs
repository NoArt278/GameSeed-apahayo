using UnityEngine;

[RequireComponent(typeof(CatBehaviourManager))]
[RequireComponent(typeof(StrayCatBehaviour))]
[RequireComponent(typeof(HidingCatBehaviour))]
public class CatBehaviourManager : MonoBehaviour {
    private ArmyCatBehaviour armyCatBehaviour;
    private StrayCatBehaviour strayCatBehaviour;
    private HidingCatBehaviour hidingCatBehaviour;

    private void Awake() {
        armyCatBehaviour = GetComponent<ArmyCatBehaviour>();
        strayCatBehaviour = GetComponent<StrayCatBehaviour>();
        hidingCatBehaviour = GetComponent<HidingCatBehaviour>();

        BecomeStrayCat();
    }

    public void BecomeStrayCat() {
        armyCatBehaviour.enabled = false;
        strayCatBehaviour.enabled = true;
        hidingCatBehaviour.enabled = false;
    }

    public void BecomeArmyCat() {
        armyCatBehaviour.enabled = true;
        strayCatBehaviour.enabled = false;
        hidingCatBehaviour.enabled = false;
    }

    public void BecomeHidingCat() {
        armyCatBehaviour.enabled = false;
        strayCatBehaviour.enabled = false;
        hidingCatBehaviour.enabled = true;
    }
}