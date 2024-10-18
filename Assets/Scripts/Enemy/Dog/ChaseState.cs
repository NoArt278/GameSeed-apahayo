using System.Collections;
using UnityEngine;

public class ChaseState : DogState
{
    private bool _isPlayerHiding;
    private bool _isPlayerLost;
    [SerializeField] private float _chaseRange;
    [SerializeField] private float _chaseAngle;
    public bool ShouldChase = false;
    private bool _lostTimerStarted = false;
    private CameraShake _cameraShake;

    private float _prevAngle;
    [SerializeField] private LayerMask _obstacleMask;

    private void Start()
    {
        _cameraShake = GameObject.FindWithTag("MainCamera").GetComponentInChildren<CameraShake>();
        if(_cameraShake == null)
        {
            print("Camera Shake Script Not Found!");
        }
    }

    private void AlignOrientation()
    {
        if (Agent.velocity.sqrMagnitude > 0.1f) SpriteRenderer.flipX = Agent.velocity.x >= 0;
    }

    public override void EnterState(DogStateMachine stateMachine)
    {
        if(Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        Agent.destination = Player.transform.position;
        ShouldChase = true;
        _prevAngle = FieldOfView.Angle;
        FieldOfView.Angle = 360;
        FieldOfView.IsChasing = true;
        Animator.SetBool("Chase", true);
        print("Enter Chase State");
        _cameraShake.StartShaking();

        GameplayUI.Instance.StartDogChase();
        AudioManager.Instance.PlayOneShot("Caught");
    }

    public override void UpdateState(DogStateMachine stateMachine)
    {
        // Start countdown if player is lost or hiding
        if (ShouldChase && FieldOfView.IsPlayerVisible && Player.GetComponent<PlayerMovement>().IsHiding() && !_lostTimerStarted)
        {
            _lostTimerStarted = true;
            _isPlayerHiding = true;
            StartCoroutine(TargetHidingRoutine());
            return;
        } 
        else if (ShouldChase && !FieldOfView.IsPlayerVisible && !_lostTimerStarted)
        {
            _lostTimerStarted = true;
            _isPlayerLost = true;
            StartCoroutine(TargetLostRoutine());
            return;
        }

        // Check if player is still lost or hiding during countdown
        if(_lostTimerStarted && _isPlayerHiding && !Player.GetComponent<PlayerMovement>().IsHiding())
        {

            Animator.SetBool("Chase", true);
            StopAllCoroutines();
            _isPlayerHiding = false;
            _lostTimerStarted = false;
            ShouldChase = true;
        }
        if(_lostTimerStarted && _isPlayerLost && FieldOfView.IsPlayerVisible)
        {
            StopAllCoroutines();
            _isPlayerLost = false;
            _lostTimerStarted = false;
            ShouldChase = true;
        }

        


        if (ShouldChase)
        {
            _cameraShake.StartShaking();
            if (_isPlayerHiding && FieldOfView.IsPlayerVisible)
            {
                Agent.destination = transform.position;
                Animator.SetBool("Chase", false);
            }
            else
            {
                Agent.destination = Player.transform.position;
            }
            FieldOfView.SetVisionDirection(transform.position, Player.transform.position);
            AlignOrientation();
        }
        else
        {
            stateMachine.ChangeState(stateMachine.PatrolState);
        }
    }

    public override void ExitState(DogStateMachine stateMachine)
    {
        Animator.SetBool("Chase", false);
        Agent.ResetPath();
        FieldOfView.Angle = _prevAngle;
        FieldOfView.IsChasing = false;
        _isPlayerLost = false;
        _isPlayerHiding = false;
        _cameraShake.StopShaking();

        GameplayUI.Instance.StopDogChase();
    }

    private IEnumerator TargetLostRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(2);

        yield return delay;

        ShouldChase = FieldOfView.IsPlayerVisible;

        _lostTimerStarted = false;      
    }

    private IEnumerator TargetHidingRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(2);

        yield return delay;

        ShouldChase = !Player.GetComponent<PlayerMovement>().IsHiding();
        print("Hiding routine finished. should chase: " + ShouldChase);

        _lostTimerStarted = false;
    }


}
