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
    private bool delayPassed = false;

    [SerializeField] private PlayerLaser playerLaser;
    [SerializeField] private Transform catFloatCenter;
    [SerializeField] private Transform staffPosition;
    [SerializeField] private float distance;

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
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
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
                if (catArmy.GetCatCount() > 0 && !currNPC.CheckCrazed())
                {
                    if (lastHypnotizedNPC == null)
                    {
                        lastHypnotizedNPC = currNPC;
                        lastHypnotizedNPC.isControllingBar = true;
                        HypnotizedNPCTr = currNPC.transform;
                        playerLaser.EmitLaser(StaffPosition, HypnotizedNPCTr);
                        GameplayUI.Instance.StartHypnotize();
                        Vector3 direction = (currNPC.Center.position - transform.position).normalized;
                        catArmy.StartHypnotize(catFloatCenter.position + direction * distance);
                        currNPC.StartHyponotize();
                        if (!catGodAnimator.GetBool("isHypnotizing"))
                        {
                            catGodAnimator.SetTrigger("StartHypnotize");
                            catGodAnimator.SetBool("isHypnotizing", true);
                        }
                    }
                }
                else if (catArmy.GetCatCount() <= 0 && !currNPC.CheckCrazed())
                {
                    currNPC.animator.SetBool("isHypno", false);
                    if(currNPC.currentState != currNPC.STATE_RANDOMMOVE){
                        currNPC.TransitionToState(currNPC.STATE_RANDOMMOVE);                         
                    }

                    AudioManager.Instance.PlayOneShot("Cant");
                    GameplayUI.Instance.ShowMainHintText("You have no cat!");
                }
                if (!currNPC.CheckCrazed() && currNPC == lastHypnotizedNPC && catArmy.GetCatCount() > 0)
                {
                    playerMovement.DisableMove();
                    if (!catGodAnimator.GetBool("isHypnotizing"))
                    {
                        catGodAnimator.SetTrigger("StartHypnotize");
                        catGodAnimator.SetBool("isHypnotizing", true);
                    }
                    lastClickTime = Time.time;
                    AudioManager.Instance.PlayOneShot("ClickHypno");
                    GameplayUI.Instance.PlayCrosshairBeat();
                    if (delayPassed) {
                        Vector3 direction = (currNPC.Center.position - transform.position).normalized;
                        catArmy.CancelHypnotize();
                        catArmy.StartHypnotize(catFloatCenter.position + direction * distance);
                        delayPassed = false;
                    }
                    currNPC.OnNPCClicked();
                }
            }
        } else
        {
            currNPC = null;
        }

        if (Time.time - lastClickTime > clickMoveDelay && !playerMovement.IsHiding())
        {
            catGodAnimator.SetBool("isHypnotizing", false);
            playerMovement.EnableMove();
            if (lastHypnotizedNPC != null)
            {
                playerLaser.StopLaser();
                catArmy.CancelHypnotize();
                delayPassed = true;
            }
        }
        else if (playerMovement.IsHiding())
        {
            lastClickTime = Time.time;
        }

        if (lastHypnotizedNPC != null)
        {
            if (lastHypnotizedNPC.CheckCrazed())
            {
                score += Mathf.RoundToInt(lastHypnotizedNPC.HypnotizeStats.hypnotizeHealth * 10);
                catArmy.DestroyCat(lastHypnotizedNPC.transform.position + Vector3.up * 0.5f);
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
    }
}
