using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class CatSpawner : MonoBehaviour
{
    private int catsInSceneCount = 0;

    [SerializeField] private GameObject _catPrefab;
    [SerializeField] private int _maxCatsInScene = 25;

    [Header("Spawn")]
    [SerializeField] private float _spawnDelay = 10f;
    [SerializeField] private int _bulkSpawnRate = 4;
    [SerializeField] private int _initialSpawn = 12;
    [SerializeField] private int _maxLocationSearchAttempts = 30;
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private Transform[] _fixedSpawnPoints;

    private LayerMask _obstacleMask;
    private BoxCollider _spawnArea;
    private Coroutine _spawnRoutine;
    private bool _firstSpawn = true;

    [SerializeField, ReadOnly] private ObjectPool _strayCatPool;
    [SerializeField] private GameObject _pulseVFX;

    private void Awake() {
        _spawnArea = GetComponent<BoxCollider>();
        _obstacleMask = LayerMask.GetMask("Obstacle");
    }

    private void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        catsInSceneCount = 0;
    }

    [Button]
    private void SerializeObjectPool() {
        _strayCatPool = new ObjectPool(new[]{ _catPrefab }, 50, _spawnParent);
    }

    [Button]
    private void ClearObjectPool() {
        while (_spawnParent.childCount > 0) {
            DestroyImmediate(_spawnParent.GetChild(0).gameObject);
        }

        _strayCatPool = null;
    }

    private void OnDisable() {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState prev, GameState current) {
        if (current == GameState.InGame) {
            _spawnRoutine = StartCoroutine(SpawnRoutine());
        } else {
            if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);
        }
    }

    private void FixedSpawn() {
        for (int i = 0; i < _fixedSpawnPoints.Length; i++) {
            GameObject cat = _strayCatPool.GetObject();
            NavMeshAgent agent = cat.GetComponent<NavMeshAgent>();

            agent.Warp(_fixedSpawnPoints[i].position);

            CatStateMachine stm = cat.GetComponent<CatStateMachine>();

            stm.Spawner = this;
            catsInSceneCount++;

            DOVirtual.DelayedCall(UnityEngine.Random.Range(0f, 0.5f), (TweenCallback)(() => {
                GameObject pulse = Instantiate((GameObject)this._pulseVFX, cat.transform);
                Action<CatBaseState, CatBaseState> PulseRemover = (prev, current) => {
                    if (current == stm.STATE_FOLLOW) {
                        Destroy(pulse);
                    }
                };

                stm.OnStateChanged += (prev, current) => {
                    if (current != stm.STATE_FOLLOW) return;
                    PulseRemover(prev, current);
                    stm.OnStateChanged -= PulseRemover;
                };
            }));
        }
    }

    private IEnumerator SpawnRoutine() {
        while (true) {
            if (_firstSpawn) {
                FixedSpawn();
                for (int i = 0; i < _initialSpawn; i++) {
                    Spawn();
                    yield return null;
                }

                _firstSpawn = false;
                yield return new WaitForSeconds(_spawnDelay);
            }

            for (int i = 0; i < _bulkSpawnRate; i++) {
                Spawn();
                yield return null;
            }

            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private void Spawn() {
        if (catsInSceneCount >= _maxCatsInScene) return;
        if (!Camera.main) return;

        Vector3 spawnPosition = GetSpawnPosition();
        if (spawnPosition == Vector3.zero) { return; }

        GameObject cat = _strayCatPool.GetObject();
        NavMeshAgent agent = cat.GetComponent<NavMeshAgent>();

        agent.Warp(spawnPosition);

        CatStateMachine stm = cat.GetComponent<CatStateMachine>();
        stm.Spawner = this;

        catsInSceneCount++;
    }

    public void Return(GameObject cat) {
        catsInSceneCount--;
        _strayCatPool.ReturnObject(cat);
    }

    private Vector3 GetSpawnPosition() {
        for (int i = 0; i < _maxLocationSearchAttempts; i++) {
            Vector3 spawnPosition = transform.position;
            Vector3 spawnSize = _spawnArea.size;

            spawnPosition.x += UnityEngine.Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
            spawnPosition.z += UnityEngine.Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas)) {
                Vector3 hitPosition = hit.position;
                if (hit.position.y > 0.3f) continue;

                // Debug.DrawRay(hitPosition, Vector3.up * 100f, Color.green, 100f);

                // CASE 0: It is inside a building
                if (Physics.OverlapSphere(hitPosition, 0.1f, _obstacleMask).Length > 0) continue;

                // CASE 1: Spawn position is obstructed by something (i.e. building)
                Vector3 directionToHit = hitPosition - Camera.main.transform.position;
                float distance = directionToHit.magnitude;

                if (Physics.Raycast(Camera.main.transform.position, directionToHit, distance, _obstacleMask))
                {
                    return hitPosition;
                }

                // CASE 2: It is outside of the camera view
                else
                {
                    Vector2 clipSpace = Camera.main.WorldToViewportPoint(hitPosition);
                    if (clipSpace.x < -0.1 || clipSpace.x > 1.1 || clipSpace.y < -0.1 || clipSpace.y > 1.1)
                    {
                        return hitPosition;
                    }
                }
            }
        }

        return Vector3.zero;
    }
}
