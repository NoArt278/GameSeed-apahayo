using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HypnotizeManager : MonoBehaviour
{

    private Collider capsuleCollider;
    public bool isHypnotized = false;
    public bool successHypnotize = false;
    public float hypnotizeHealth = 10f; // number of clicks to hypnotize
    [SerializeField] public Image hypnoBar;
    
    private float hypnoMeter = 0f;

    // Countdown fields
    public float countdownMultiplier = 2f; // Duration of the countdown in seconds
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
        UpdateHypnoBar();
        ClickDetector();
    }

    void UpdateHypnoBar(){
        hypnoBar.fillAmount = (float)hypnoMeter / (float)hypnotizeHealth;
    }

    void ClickDetector() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider == capsuleCollider) {
                    Debug.Log("Clicked on NPC");
                    StartCountdown();
                    isHypnotized = true;
                    if (hypnotizeHealth <= hypnoMeter){
                        successHypnotize = true;
                    } else {
                        hypnoMeter++;
                    }
                }
            }
        }
    }

    // void OnMouseDown()
    // {
    //     Debug.Log("Clicked on NPC");
    //     StartCountdown();
    //     isHypnotized = true;
    //     if (hypnotizeHealth <= hypnoMeter){
    //         successHypnotize = true;
    //     } else {
    //         hypnoMeter++;
    //     }
    // }

    void CountDown(){ // check if the countdown is over
        if(isCountdownActive){
            hypnoMeter -= (Time.deltaTime * countdownMultiplier);
            if(hypnoMeter <= 0){
                CountdownEnd();
            }
        }
    }

    void StartCountdown(){
        isCountdownActive = true;
    }

    void CountdownEnd(){
        isCountdownActive = false;
        isHypnotized = false;
        Debug.Log("Hypnotize effect has ended");
    }

    public void SetupHypnoBar(Canvas canvas){
        hypnoBar.transform.SetParent(canvas.transform);
    }
}
