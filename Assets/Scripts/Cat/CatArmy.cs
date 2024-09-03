using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class CatArmy : MonoBehaviour
{
    // [SerializeField] private Transform follow;
    private Transform follow;

    [ReadOnly] [SerializeField] private List<ArmyCatBehaviour> cats;
    [SerializeField] private GameObject armyCatPrefab;

    private float catRadius;
    private List<ArmyCatBehaviour> outsideCats = new();

    private void Awake() {
        // foreach (ArmyCatBehaviour cat in cats) {
        //     cat.Initialize(follow);
        // }

        catRadius = armyCatPrefab.GetComponent<NavMeshAgent>().radius;
    }
    
    [Button]
    public void AddCat()
    {
        // GameObject newCat = Instantiate(armyCatPrefab, transform);
        // newCat.transform.position = FindAppropriateSpawnLocation();
        // ArmyCatBehaviour catBehaviour = newCat.GetComponent<ArmyCatBehaviour>();
        // catBehaviour.Initialize(follow);
        // cats.Add(catBehaviour);
    }

    public bool RegisterCat(ArmyCatBehaviour cat, Transform follow) 
    {
        if (cats.Contains(cat)) return false;

        this.follow = follow == null ? this.follow : follow;

        cat.Initialize(follow, follow.GetComponent<PlayerStats>().walkSpeed);
        cat.GetComponent<CatBehaviourManager>().BecomeArmyCat();

        cats.Add(cat);
        RecalculateStoppingDistance();

        return true;
    }

    private void RecalculateStoppingDistance() {
        if (cats.Count == 0) return;

        float stoppingDistance;
        switch (cats.Count) {
            case <= 7:
                stoppingDistance = catRadius * 2 + 0.02f;
                break;
            case <= 21:
                stoppingDistance = (catRadius * 2 + 0.02f) * 2;
                break;
            case <= 42:
                stoppingDistance = (catRadius * 2 + 0.02f) * 3;
                break;
            default:
                stoppingDistance = (catRadius * 2 + 0.02f) * 4;
                break;
        }

        if (stoppingDistance == cats[0].GetComponent<NavMeshAgent>().stoppingDistance) return;

        foreach (ArmyCatBehaviour armyCat in cats) {
            armyCat.GetComponent<NavMeshAgent>().stoppingDistance = stoppingDistance;
        }
    }

    public int GetCatCount()
    {
        return cats.Count;
    }

    public void HideCats(Vector3 hidePosition) {
        foreach (ArmyCatBehaviour cat in cats) {
            cat.GetComponent<CatBehaviourManager>().BecomeHidingCat();
            cat.GetComponent<HidingCatBehaviour>().StartHiding(hidePosition);
        }
    }

    public void QuitHiding(Vector3 exitPosition) {
        for (int i = 0; i < cats.Count; i++) {
            ArmyCatBehaviour cat = cats[i];
            if (i == 0) {
                outsideCats.Clear();
                cat.GetComponent<HidingCatBehaviour>().QuitHiding(
                    exitPosition,
                    onQuitComplete: () => {
                        cat.GetComponent<CatBehaviourManager>().BecomeArmyCat();
                    }
                );
                outsideCats.Add(cat);
                continue;
            }

            cat.GetComponent<HidingCatBehaviour>().QuitHiding(
                FindAppropriateSpawnLocation(),
                onQuitComplete: () => {
                    cat.GetComponent<CatBehaviourManager>().BecomeArmyCat();
                }
            );
            outsideCats.Add(cat);
        }

        // foreach (ArmyCatBehaviour cat in cats) {
        //     cat.GetComponent<HidingCatBehaviour>().QuitHiding(
        //         exitPosition,
        //         onQuitComplete: () => cat.GetComponent<CatBehaviourManager>().BecomeArmyCat()
        //     );
        // }
    }

    public void StartSprint(float speed) {
        foreach (ArmyCatBehaviour cat in cats) {
            cat.Sprint(speed);
        }
    }

    public void StopSprint() {
        foreach (ArmyCatBehaviour cat in cats) {
            cat.StopSprint();
        }
    }

    public void FleeOuterCat() {
        if (cats.Count == 0) return;

        ArmyCatBehaviour outerCat = cats[0];
        float maxDistance = Vector3.Distance(outerCat.transform.position, follow.position);
        foreach (ArmyCatBehaviour cat in cats) {
            float distance = Vector3.Distance(cat.transform.position, follow.position);
            if (distance > maxDistance) {
                outerCat = cat;
                maxDistance = distance;
            }
        }

        outerCat.Flee();
        cats.Remove(outerCat);
    }

    private Vector3 FindAppropriateSpawnLocation()
    {
        if (follow == null) return Vector3.zero;

        if (outsideCats.Count == 0)
        {
            return follow.position + 2 * catRadius * Random.insideUnitSphere;
        }

        Vector3 averagePosition = Vector3.zero;
        foreach (ArmyCatBehaviour cat in outsideCats)
        {
            averagePosition += cat.transform.position;
        }
        averagePosition /= outsideCats.Count;

        Vector3 followDirection = (follow.position - averagePosition).normalized;
        if (followDirection == Vector3.zero)
        {
            followDirection = Random.insideUnitSphere;
        }

        return averagePosition + 2 * catRadius * followDirection;
    }
}