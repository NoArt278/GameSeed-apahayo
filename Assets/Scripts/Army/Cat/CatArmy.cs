using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class CatArmy : MonoBehaviour
{
    private Transform follow;

    [ReadOnly, SerializeField] private List<CatStateMachine> _cats;
    [SerializeField] private GameObject _armyCatPrefab;

    private float _catRadius;
    private readonly List<CatStateMachine> _outsideCats = new();
    private CatStateMachine _usedCat;

    private void Awake() {
        _catRadius = _armyCatPrefab.GetComponent<NavMeshAgent>().radius;
    }

    public bool RegisterCat(CatStateMachine cat, Transform player) 
    {
        if (_cats.Contains(cat)) return false;

        follow = player == null ? follow : player;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        cat.BecomeFollower(this, player, stats.WalkSpeed, stats.SprintSpeed);

        _cats.Add(cat);
        _outsideCats.Add(cat);
        RecalculateStoppingDistance();

        return true;
    }

    private void RecalculateStoppingDistance() {
        if (_cats.Count == 0) return;

        float stoppingDistance;
        switch (_cats.Count) {
            case <= 7:
                stoppingDistance = _catRadius * 2 + 0.02f;
                break;
            case <= 21:
                stoppingDistance = (_catRadius * 2 + 0.02f) * 2;
                break;
            case <= 42:
                stoppingDistance = (_catRadius * 2 + 0.02f) * 3;
                break;
            default:
                stoppingDistance = (_catRadius * 2 + 0.02f) * 4;
                break;
        }

        if (stoppingDistance == _cats[0].GetComponent<NavMeshAgent>().stoppingDistance) return;

        foreach (CatStateMachine cat in _cats) {
            cat.Agent.stoppingDistance = stoppingDistance;
        }
    }

    public int GetCatCount()
    {
        return _cats.Count;
    }

    public void HideCats(Vector3 hidePosition) {
        foreach (CatStateMachine cat in _cats) {
            cat.GoHiding(hidePosition);
        }
    }

    public void StartHypnotize(Vector3 catFloatPos) {
        if (_cats.Count == 0) return;
        if (_usedCat != null) return;

        Debug.Log("Hypnotize");

        _usedCat = _cats[0];
        _usedCat.UseCatForHypnotize(catFloatPos);
    }

    public void CancelHypnotize() {
        if (_cats.Count == 0) return;
        if (_usedCat == null) return;

        if (!follow) {
            follow = GameObject.FindGameObjectWithTag("Player").transform;
        }

        _usedCat.CancelHypnotize(FindAppropriateSpawnLocation(follow.position));
        _usedCat = null;
    }

    public void DestroyCat(Vector3 endPosition)
    {
        if (_cats.Count == 0) return;
        if (_usedCat == null) return;

        Debug.Log("Destroy");

        _cats.Remove(_usedCat);
        _outsideCats.Remove(_usedCat);
        _usedCat.STATE_HYPNOTIZE.ResetCat();
        _usedCat.transform.DOMove(endPosition, 0.2f).OnComplete(() => {
            _usedCat.ReturnToSpawner();
            GameplayUI.Instance.UpdateCatCount(GetCatCount());
            _usedCat = null;
        }).SetEase(Ease.Linear);

    }

    public void RemoveCat(CatStateMachine cat)
    {
        if (_cats.Count == 0) return;

        _cats.Remove(cat);
        _outsideCats.Remove(cat);
        if (_usedCat == cat) _usedCat = null;
        GameplayUI.Instance.UpdateCatCount(GetCatCount());
    }

    public void QuitHiding(Vector3 exitPosition) {
        for (int i = 0; i < _cats.Count; i++) {
            CatStateMachine cat = _cats[i];
            if (i == 0) {
                _outsideCats.Clear();
                cat.QuitHiding( FindAppropriateSpawnLocation(exitPosition) );
                _outsideCats.Add(cat);
                continue;
            }

            cat.QuitHiding( FindAppropriateSpawnLocation(exitPosition) );
            _outsideCats.Add(cat);
        }
    }

    public void StartSprint() {
        foreach (CatStateMachine cat in _cats) {
            cat.StartSprint();
        }
    }

    public void StopSprint() {
        foreach (CatStateMachine cat in _cats) {
            cat.StopSprint();
        }
    }

    public Vector3 FindAppropriateSpawnLocation(Vector3 center)
    {
        Vector3 spawnPosition = center + 3 * _catRadius * Random.insideUnitSphere;
        NavMeshHit hit;
        if (_outsideCats.Count == 0)
        {
            if (NavMesh.SamplePosition(spawnPosition, out hit, 10, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return spawnPosition;
        }

        Vector3 averagePosition = Vector3.zero;
        foreach (CatStateMachine cat in _outsideCats)
        {
            averagePosition += cat.transform.position;
        }
        averagePosition /= _outsideCats.Count;

        Vector3 followDirection = (follow.position - averagePosition).normalized;
        if (followDirection == Vector3.zero)
        {
            followDirection = Random.insideUnitSphere;
        }
        followDirection.y = 0;

        spawnPosition = averagePosition + 3 * _catRadius * followDirection;
        if (NavMesh.SamplePosition(spawnPosition, out hit, 10, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return spawnPosition;
    }
}