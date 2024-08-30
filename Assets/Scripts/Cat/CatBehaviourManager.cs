using UnityEngine;

[RequireComponent(typeof(CatBehaviourManager))]
[RequireComponent(typeof(StrayCatBehaviour))]
[RequireComponent(typeof(HidingCatBehaviour))]
public class CatBehaviourManager : MonoBehaviour {
    private ArmyCatBehaviour armyCatBehaviour;
    private StrayCatBehaviour strayCatBehaviour;
    private HidingCatBehaviour hidingCatBehaviour;

    public enum State { Stray, Army, Hiding }
    public State CurrentState { get; private set; } = State.Stray;

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

        CurrentState = State.Stray;
    }

    public void BecomeArmyCat() {
        armyCatBehaviour.enabled = true;
        strayCatBehaviour.enabled = false;
        hidingCatBehaviour.enabled = false;

        CurrentState = State.Army;
    }

    public void BecomeHidingCat() {
        armyCatBehaviour.enabled = false;
        strayCatBehaviour.enabled = false;
        hidingCatBehaviour.enabled = true;

        CurrentState = State.Hiding;
    }
}