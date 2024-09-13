using System.Collections;
using UnityEngine;

public class ChaseState : DogState
{
    private bool isPlayerHiding;
    private bool isPlayerLost;
    [SerializeField] private float chaseRange;
    [SerializeField] private float chaseAngle;
    public bool shouldChase = false;
    private bool lostTimerStarted = false;
    private CameraShake cameraShake;

    private float prevAngle;
    [SerializeField] private LayerMask obstacleMask;

    private void Start()
    {
        cameraShake = GameObject.FindWithTag("MainCamera").GetComponentInChildren<CameraShake>();
        if(cameraShake == null)
        {
            print("Camera Shake Script Not Found!");
        }
    }

    private void AlignOrientation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f) spriteRenderer.flipX = agent.velocity.x >= 0;
    }

    public override void EnterState(DogStateMachine stateMachine)
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        agent.destination = player.transform.position;
        shouldChase = true;
        prevAngle = fieldOfView.Angle;
        fieldOfView.Angle = 360;
        fieldOfView.isChasing = true;
        animator.SetBool("Chase", true);
        print("Enter Chase State");
        cameraShake.isChased = true;

        GameplayUI.Instance.StartDogChase();
    }

    public override void UpdateState(DogStateMachine stateMachine)
    {
        // Start countdown if player is lost or hiding
        if (shouldChase && fieldOfView.isPlayerVisible && player.GetComponent<PlayerMovement>().IsHiding() && !lostTimerStarted)
        {
            lostTimerStarted = true;
            isPlayerHiding = true;
            StartCoroutine(TargetHidingRoutine());
            return;
        } 
        else if (shouldChase && !fieldOfView.isPlayerVisible && !lostTimerStarted)
        {
            lostTimerStarted = true;
            isPlayerLost = true;
            StartCoroutine(TargetLostRoutine());
            return;
        }

        // Check if player is still lost or hiding during countdown
        if(lostTimerStarted && isPlayerHiding && !player.GetComponent<PlayerMovement>().IsHiding())
        {

            animator.SetBool("Chase", true);
            StopAllCoroutines();
            isPlayerHiding = false;
            lostTimerStarted = false;
            shouldChase = true;
        }
        if(lostTimerStarted && isPlayerLost && fieldOfView.isPlayerVisible)
        {
            StopAllCoroutines();
            isPlayerLost = false;
            lostTimerStarted = false;
            shouldChase = true;
        }

        


        if (shouldChase)
        {
            cameraShake.isChased = true;
            if (isPlayerHiding && fieldOfView.isPlayerVisible)
            {
                agent.destination = transform.position;
                animator.SetBool("Chase", false);
            }
            else
            {
                agent.destination = player.transform.position;
            }
            fieldOfView.SetVisionDirection(transform.position, player.transform.position);
            AlignOrientation();
        }
        else
        {
            stateMachine.ChangeState(stateMachine.PatrolState);
        }
    }

    public override void ExitState(DogStateMachine stateMachine)
    {
        animator.SetBool("Chase", false);
        agent.ResetPath();
        fieldOfView.Angle = prevAngle;
        fieldOfView.isChasing = false;
        isPlayerLost = false;
        isPlayerHiding = false;
        cameraShake.isChased = false;

        GameplayUI.Instance.StopDogChase();
    }

    private IEnumerator TargetLostRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(2);

        yield return delay;

        shouldChase = fieldOfView.isPlayerVisible;

        lostTimerStarted = false;      
    }

    private IEnumerator TargetHidingRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(2);

        yield return delay;

        shouldChase = !player.GetComponent<PlayerMovement>().IsHiding();
        print("Hiding routine finished. should chase: " + shouldChase);

        lostTimerStarted = false;
    }


}
