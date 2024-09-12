using UnityEngine;

public class PlayerHypnotize : MonoBehaviour
{
    private CatArmy catArmy;
    private NPCStateMachine currNPC, lastHypnotizedNPC, lastCrazedNPC;
    private PlayerMovement playerMovement;
    private SpriteRenderer sr;
    private Animator catGodAnimator;
    private float lastClickTime;
    private const float clickMoveDelay = 0.5f;
    private int score = 0;

    [SerializeField] private Transform catFloatPosition;

    private void Awake()
    {
        catArmy = GetComponent<CatArmy>();
        playerMovement = GetComponent<PlayerMovement>();
        catGodAnimator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        lastClickTime = 0;
        GameplayUI.Instance.UpdateScore(score);
    }

    void Update()
    {
        if (playerMovement.IsHiding() || playerMovement.IsDead())
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.SphereCast(ray.origin, 1, ray.direction, out hit, Mathf.Infinity, LayerMask.GetMask("NPC")))
        {
            currNPC = hit.collider.GetComponent<NPCStateMachine>();
            if (InputContainer.playerInputs.Player.Fire.WasPerformedThisFrame())
            {
                sr.flipX = currNPC.transform.position.x < transform.position.x;
                if (catArmy.GetCatCount() > 0 && !currNPC.CheckHypnotize() && !currNPC.CheckCrazed())
                {
                    currNPC.StartHyponotize();
                    if (lastHypnotizedNPC != null)
                    {
                        lastHypnotizedNPC.isControllingBar = false;
                    }
                    lastHypnotizedNPC = currNPC;
                    lastHypnotizedNPC.isControllingBar = true;
                    catGodAnimator.SetTrigger("StartHypnotize");
                    catGodAnimator.SetBool("isHypnotizing", true);
                    GameplayUI.Instance.StartHypnotize();

                    // Vector3 catFloatPos = catFloatPosition.position;
                    // if (sr.flipX) catFloatPos.x *= -1;
                    // catArmy.StartHypnotize(catFloatPos);
                }
                else if (catArmy.GetCatCount() <= 0 && !currNPC.CheckCrazed())
                {
                    currNPC.animator.SetBool("isHypno", false);
                    currNPC.TransitionToState(currNPC.STATE_RANDOMMOVE);
                }
                else if (currNPC != lastHypnotizedNPC && currNPC.CheckHypnotize())
                {
                    if (lastHypnotizedNPC != null)
                    {
                        lastHypnotizedNPC.isControllingBar = false;
                    }
                    lastHypnotizedNPC = currNPC;
                    lastHypnotizedNPC.isControllingBar = true;
                }
                if (!currNPC.CheckCrazed() && catArmy.GetCatCount() > 0)
                {
                    playerMovement.DisableMove();
                    lastClickTime = Time.time;
                }
            }
        } else
        {
            currNPC = null;
        }

        if (lastHypnotizedNPC != null)
        {
            if (lastHypnotizedNPC.CheckCrazed())
            {
                score += Mathf.RoundToInt(lastHypnotizedNPC.HypnotizeStats.hypnotizeHealth * 10);
                catArmy.DestroyCat();
                GameplayUI.Instance.UpdateCatCount(catArmy.GetCatCount());
                GameplayUI.Instance.UpdateScore(score);
                GameplayUI.Instance.StopHypnotize();
                lastHypnotizedNPC.isControllingBar = false;
                lastHypnotizedNPC = null;
            }
            else if (!lastHypnotizedNPC.CheckHypnotize())
            {
                GameplayUI.Instance.StopHypnotize();
                lastHypnotizedNPC.isControllingBar = false;
                lastHypnotizedNPC = null;
            }
        }

        if (Time.time - lastClickTime > clickMoveDelay && !playerMovement.IsHiding())
        {
            catGodAnimator.SetBool("isHypnotizing", false);
            playerMovement.EnableMove();
        }
        else if (playerMovement.IsHiding())
        {
            lastClickTime = Time.time;
        }
    }
}
