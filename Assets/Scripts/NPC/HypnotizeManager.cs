using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypnotizeManager : MonoBehaviour
{

    private Collider capsuleCollider;
    public bool isHypnotized = false;
    public int hypnotizeHealth = 5;

    // Countdown fields
    public float countdownDuration = 1f; // Duration of the countdown in seconds
    private float timeRemaining;
    private bool isCountdownActive = false;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        CountDown();   
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked on NPC");
        StartCountdown();
        isHypnotized = true;
        hypnotizeHealth--;
        if (hypnotizeHealth <= 0){
            // TODO: Add hypnotize state change
        }
    }

    void CountDown(){
        if(isCountdownActive){
            timeRemaining -= Time.deltaTime;
            if(timeRemaining <= 0){
                CountdownEnd();
            }
        }
    }

    void StartCountdown(){
        timeRemaining = countdownDuration;
        isCountdownActive = true;
    }

    void CountdownEnd(){
        isCountdownActive = false;  
        isHypnotized = false;
        Debug.Log("Hypnotize effect has ended");
    }
}
