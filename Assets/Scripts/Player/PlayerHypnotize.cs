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

    [SerializeField] private Transform catFloatCenter;
    [SerializeField] private float distance;
    [SerializeField] private PlayerLaser playerLaser;
    [SerializeField] private Transform staffPosition;

    private Vector3 originalStaffPosition;

    public Transform StaffPosition { get {
        Vector3 stPos = originalStaffPosition;
        stPos.x *= sr.flipX ? -1 : 1;

        staffPosition.localPosition = stPos;
        return staffPosition;
    }}
    
    public Transform HypnotizedNPCTr { get; private set; }

    private void Awake()
    {
        catArmy = GetComponent<CatArmy>();
        playerMovement = GetComponent<PlayerMovement>();
        catGodAnimator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        lastClickTime = 0;
        GameplayUI.Instance.UpdateScore(score);

        originalStaffPosition = staffPosition.localPosition;
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
                    Debug.Log("Start Hypnotize");
                    currNPC.StartHyponotize();
                    if (lastHypnotizedNPC != null)
                    {
                        lastHypnotizedNPC.isControllingBar = false;
                    }
                    lastHypnotizedNPC = currNPC;
                    lastHypnotizedNPC.isControllingBar = true;
                    HypnotizedNPCTr = currNPC.transform;
                    catGodAnimator.SetTrigger("StartHypnotize");
                    catGodAnimator.SetBool("isHypnotizing", true);
                    GameplayUI.Instance.StartHypnotize();

                    Vector3 direction = (currNPC.transform.position - transform.position).normalized;
                    catArmy.StartHypnotize(catFloatCenter.position + direction * distance);
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
                    HypnotizedNPCTr = currNPC.transform;
                }
                if (!currNPC.CheckCrazed() && catArmy.GetCatCount() > 0)
                {
                    playerMovement.DisableMove();
                    lastClickTime = Time.time;
                    AudioManager.Instance.PlayOneShot("ClickHypno");
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
                AudioManager.Instance.PlayOneShot("HypnoSuccess");
                playerLaser.StopLaser();
                lastHypnotizedNPC.isControllingBar = false;
                lastHypnotizedNPC = null;
            }
            else if (!lastHypnotizedNPC.CheckHypnotize())
            {
                GameplayUI.Instance.StopHypnotize();
                catArmy.CancelHypnotize();
                playerLaser.StopLaser();
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
            AudioManager.Instance.PlayOneShot("ClickHypno");
        }
    }
}
