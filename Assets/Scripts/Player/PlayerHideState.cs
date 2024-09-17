using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHideState : PlayerBaseState {
    private Animator hideSpotAnim;
    private CinemachineTransposer transposer;
    private Transform hideSpriteTr;

    public PlayerHideState(PlayerStateMachine stm) : base(stm) { }

    public override void EnterState() {
        transposer = stm.VCam.GetCinemachineComponent<CinemachineTransposer>();

        transposer.m_XDamping = 2;
        transposer.m_YDamping = 2;
        transposer.m_ZDamping = 2;

        hideSpotAnim = stm.ClosestHideSpot.GetComponentInChildren<Animator>();
        hideSpriteTr = stm.ClosestHideSpot.GetComponentInChildren<SpriteRenderer>().transform;

        stm.CC.enabled = false;
        stm.transform.position = stm.ClosestHideSpot.position + stm.ClosestHideSpot.forward;
        stm.CC.enabled = true;

        InputContainer.playerInputs.Player.Interact.started += Unhide;
        VFXManager.Instance.PlayPoofVFX(stm.transform.position);

        stm.StopAllCoroutines();
        stm.StartCoroutine(HideCor());
    }

    private IEnumerator HideCor() {
        yield return new WaitForSeconds(0.3f);

        stm.CapsuleCollider.enabled = false;
        stm.PlayerRenderer.enabled = false;
        GameplayUI.Instance.ChangeHideText("(E) Unhide");

        stm.Army.HideCats(stm.ClosestHideSpot.position);

        hideSpotAnim.SetBool("isHiding", true);
        hideSpriteTr.LookAt(new Vector3(hideSpriteTr.position.x, hideSpriteTr.position.y, hideSpriteTr.position.z + 5));

        if (Mathf.RoundToInt(hideSpriteTr.localRotation.eulerAngles.y) == 90 || Mathf.RoundToInt(hideSpriteTr.localRotation.eulerAngles.y) == -90
                || Mathf.RoundToInt(hideSpriteTr.localRotation.eulerAngles.y) == 270)
        {
            hideSpriteTr.localScale = new Vector3(hideSpriteTr.localScale.z, hideSpriteTr.localScale.y, hideSpriteTr.localScale.x);
        }
    }

    private void Unhide(InputAction.CallbackContext _) {
        stm.ChangeState(stm.STATE_MOVE);
    }

    public override void ExitState() {
        stm.transform.position = stm.ClosestHideSpot.position + stm.ClosestHideSpot.forward;
        stm.PlayerRenderer.enabled = true;

        Vector3 exitPos = stm.ClosestHideSpot.GetComponent<TrashBin>().GroundPoint.position + stm.ClosestHideSpot.forward * 1.5f;
        stm.Army.QuitHiding(exitPos);
        hideSpotAnim.SetBool("isHiding", false);

        stm.StopAllCoroutines();
        stm.StartCoroutine(UnhideCor());
    }

    private IEnumerator UnhideCor() {
        yield return new WaitForSeconds(0.3f);
        stm.CapsuleCollider.enabled = true;
        GameplayUI.Instance.ChangeHideText("(E) Hide");

        transposer.m_XDamping = 0;
        transposer.m_YDamping = 0;
        transposer.m_ZDamping = 0;

        if (Mathf.RoundToInt(hideSpriteTr.localRotation.eulerAngles.y) == 90 || Mathf.RoundToInt(hideSpriteTr.localRotation.eulerAngles.y) == -90
                || Mathf.RoundToInt(hideSpriteTr.localRotation.eulerAngles.y) == 270)
        {
            hideSpriteTr.localScale = new Vector3(hideSpriteTr.localScale.z, hideSpriteTr.localScale.y, hideSpriteTr.localScale.x);
        }
    }
}