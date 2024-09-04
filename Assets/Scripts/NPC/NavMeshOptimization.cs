using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshOptimization : MonoBehaviour
{
    private NavMeshAgent agent;
    public bool changeDestination = false;
    public int waitTimeMin = 2;
    public int waitTimeMax = 5;
    public float timeRemaining;
    public bool isCountingDown = false;
    public float distanceAllowed = 0.2f;
    private float distanceToDestination;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        distanceToDestination = agent.remainingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if(!changeDestination){
            StopCheck();
        }
    }

    void StopCheck()
    { 
        if(distanceToDestination - agent.remainingDistance <= distanceAllowed){
            if(!isCountingDown){
                Debug.Log("Start countdown");
                StartCountdown();
            } else {
                UpdateCountdown();
            }
        } else {
            isCountingDown = false;
        }
        
    }

    void StartCountdown()
    {
        timeRemaining = Random.Range(waitTimeMin, waitTimeMax);
        isCountingDown = true;
    }

    void UpdateCountdown()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            changeDestination = true;
            isCountingDown = false;
        }
    }



}
