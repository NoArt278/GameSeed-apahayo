using UnityEngine;

public class PlayerHypnotize : MonoBehaviour
{
    private CatArmy catArmy;
    private NPCStateMachine currNPC, lastHypnotizedNPC, lastCrazedNPC;
    private PlayerMovement playerMovement;
    private bool isCurrHypnotized = false;
    private float lastClickTime;
    private const float clickMoveDelay = 0.5f;
    private int score = 0;

    private void Awake()
    {
        catArmy = GetComponent<CatArmy>();
        playerMovement = GetComponent<PlayerMovement>();
        lastClickTime = 0;
        PlayerUI.Instance.UpdateScore(score);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.SphereCast(ray.origin, 1, ray.direction, out hit, Mathf.Infinity, LayerMask.GetMask("NPC")))
        {
            currNPC = hit.collider.GetComponent<NPCStateMachine>();
            if (InputContainer.playerInputs.Player.Fire.WasPerformedThisFrame())
            {
                if (catArmy.GetCatCount() > 0 && !currNPC.CheckHypnotize() && !currNPC.CheckCrazed())
                {
                    currNPC.StartHyponotize();
                    lastHypnotizedNPC = currNPC;
                } else if (catArmy.GetCatCount() <= 0 && !currNPC.CheckCrazed())
                {
                    currNPC.TransitionToState(currNPC.STATE_RANDOMMOVE);
                }
                if (!currNPC.CheckCrazed() && catArmy.GetCatCount() > 0)
                {
                    playerMovement.DisableMove();
                    lastClickTime = Time.time;
                } 
            }
            if (currNPC.CheckCrazed() && currNPC == lastHypnotizedNPC && (lastCrazedNPC == null || lastCrazedNPC != currNPC))
            {
                lastCrazedNPC = currNPC;
                catArmy.DestroyCat();
                score += Mathf.RoundToInt(currNPC.HypnotizeStats.hypnotizeHealth * 10);
                PlayerUI.Instance.UpdateCatCount(catArmy.GetCatCount());
                PlayerUI.Instance.UpdateScore(score);
            }
        } else
        {
            currNPC = null;
        }

        if (Time.time - lastClickTime > clickMoveDelay && !playerMovement.IsHiding())
        {
            playerMovement.EnableMove();
        }
        else if (playerMovement.IsHiding())
        {
            lastClickTime = Time.time;
        }
    }
}
