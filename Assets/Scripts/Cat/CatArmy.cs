using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class CatArmy : MonoBehaviour
{
    private Transform follow;

    [ReadOnly] [SerializeField] private List<CatStateMachine> cats;
    [SerializeField] private GameObject armyCatPrefab;

    private float catRadius;
    private readonly List<CatStateMachine> outsideCats = new();
    private CatStateMachine usedCat;

    private void Awake() {
        catRadius = armyCatPrefab.GetComponent<NavMeshAgent>().radius;
    }

    public bool RegisterCat(CatStateMachine cat, Transform player) 
    {
        if (cats.Contains(cat)) return false;

        follow = player == null ? follow : player;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        cat.BecomeFollower(this, player, stats.walkSpeed, stats.sprintSpeed);

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

        foreach (CatStateMachine cat in cats) {
            cat.Agent.stoppingDistance = stoppingDistance;
        }
    }

    public int GetCatCount()
    {
        return cats.Count;
    }

    public void HideCats(Vector3 hidePosition) {
        foreach (CatStateMachine cat in cats) {
            cat.GoHiding(hidePosition);
        }
    }

    public void StartHypnotize(Vector3 catFloatPos) {
        if (cats.Count == 0) return;

        usedCat = cats[Random.Range(0, cats.Count)];
        usedCat.UseCatForHypnotize(catFloatPos);
    }

    public void CancelHypnotize() {
        if (cats.Count == 0) return;

        usedCat.CancelHypnotize( FindAppropriateSpawnLocation(follow ? follow.position : Vector3.zero) );
    }

    public void DestroyCat(Vector3 endPosition)
    {
        if (cats.Count == 0) return;

        cats.Remove(usedCat);
        usedCat.transform.DOMove(endPosition, 0.2f).OnComplete(() => {
            usedCat.ReturnToSpawner();
            GameplayUI.Instance.UpdateCatCount(GetCatCount());
        }).SetEase(Ease.Linear);

    }

    public void RemoveCat(CatStateMachine cat)
    {
        if (cats.Count == 0) return;

        cats.Remove(cat);
        GameplayUI.Instance.UpdateCatCount(GetCatCount());
    }

    public void QuitHiding(Vector3 exitPosition) {
        for (int i = 0; i < cats.Count; i++) {
            CatStateMachine cat = cats[i];
            if (i == 0) {
                outsideCats.Clear();
                cat.QuitHiding( FindAppropriateSpawnLocation(exitPosition) );
                outsideCats.Add(cat);
                continue;
            }

            cat.QuitHiding( FindAppropriateSpawnLocation(exitPosition) );
            outsideCats.Add(cat);
        }
    }

    public void StartSprint() {
        foreach (CatStateMachine cat in cats) {
            cat.StartSprint();
        }
    }

    public void StopSprint() {
        foreach (CatStateMachine cat in cats) {
            cat.StopSprint();
        }
    }

    private Vector3 FindAppropriateSpawnLocation(Vector3 center)
    {
        if (outsideCats.Count == 0)
        {
            return center + 2 * catRadius * Random.insideUnitSphere;
        }

        Vector3 averagePosition = Vector3.zero;
        foreach (CatStateMachine cat in outsideCats)
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