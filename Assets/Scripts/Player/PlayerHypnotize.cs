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
                    lastHypnotizedNPC = currNPC;
                    catGodAnimator.SetTrigger("StartHypnotize");
                    catGodAnimator.SetBool("isHypnotizing", true);

                    // Vector3 catFloatPos = catFloatPosition.position;
                    // if (sr.flipX) catFloatPos.x *= -1;
                    // catArmy.StartHypnotize(catFloatPos);
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
                GameplayUI.Instance.UpdateCatCount(catArmy.GetCatCount());
                GameplayUI.Instance.UpdateScore(score);
            }
        } else
        {
            currNPC = null;
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
