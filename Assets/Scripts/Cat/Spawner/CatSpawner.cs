using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class CatSpawner : MonoBehaviour
{
    private int strayCatCount = 0;
    private int catsInSceneCount = 0;

    [SerializeField] private GameObject catPrefab;
    [SerializeField] private int maxStrayCats = 20;
    [SerializeField] private int maxCatsInScene = 25;

    [Header("Spawn")]
    [SerializeField] private float spawnDelay = 10f;
    [SerializeField] private int bulkSpawnRate = 4;
    [SerializeField] private int initialSpawn = 12;
    [SerializeField] private int maxLocationSearchAttempts = 30;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Transform[] fixedSpawnPoints;

    private LayerMask obstacleMask;
    private BoxCollider spawnArea;
    private Coroutine spawnRoutine;
    private bool firstSpawn = true;

    [SerializeField, ReadOnly] private ObjectPool strayCatPool;
    [SerializeField] private GameObject pulseVFX;

    private void Awake() {
        spawnArea = GetComponent<BoxCollider>();
        obstacleMask = LayerMask.GetMask("Obstacle");
    }

    private void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        strayCatCount = 0;
        catsInSceneCount = 0;
    }

    [Button]
    private void SerializeObjectPool() {
        strayCatPool = new ObjectPool(new[]{ catPrefab }, 50, spawnParent);
    }

    [Button]
    private void ClearObjectPool() {
        while (spawnParent.childCount > 0) {
            DestroyImmediate(spawnParent.GetChild(0).gameObject);
        }

        strayCatPool = null;
    }

    private void OnDisable() {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState prev, GameState current) {
        if (current == GameState.InGame) {
            spawnRoutine = StartCoroutine(SpawnRoutine());
        } else {
            if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        }
    }

    private void FixedSpawn() {
        for (int i = 0; i < fixedSpawnPoints.Length; i++) {
            GameObject cat = strayCatPool.GetObject();
            NavMeshAgent agent = cat.GetComponent<NavMeshAgent>();

            agent.Warp(fixedSpawnPoints[i].position);

            CatStateMachine stm = cat.GetComponent<CatStateMachine>();

            stm.Spawner = this;
            strayCatCount++;
            catsInSceneCount++;


            stm.OnStateChanged += (prev, current) => {
                if (prev == stm.STATE_STRAYIDLE || prev == stm.STATE_STRAYWANDER) {
                    strayCatCount--;
                }

                if (current == stm.STATE_STRAYIDLE || current == stm.STATE_STRAYWANDER) {
                    strayCatCount++;
                }
            };
            
            DOVirtual.DelayedCall(UnityEngine.Random.Range(0f, 0.5f), () => {
                GameObject pulse = Instantiate(pulseVFX, cat.transform);
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
            });
        }
    }

    private IEnumerator SpawnRoutine() {
        while (true) {
            if (firstSpawn) {
                FixedSpawn();
                for (int i = 0; i < initialSpawn; i++) {
                    Spawn();
                    yield return null;
                }

                firstSpawn = false;
                yield return new WaitForSeconds(spawnDelay);
            }

            for (int i = 0; i < bulkSpawnRate; i++) {
                Spawn();
                yield return null;
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Spawn() {
        if (strayCatCount >= maxStrayCats) return;
        if (catsInSceneCount >= maxCatsInScene) return;
        if (!Camera.main) return;

        Vector3 spawnPosition = GetSpawnPosition();
        if (spawnPosition == Vector3.zero) { return; }

        GameObject cat = strayCatPool.GetObject();
        NavMeshAgent agent = cat.GetComponent<NavMeshAgent>();

        agent.Warp(spawnPosition);

        CatStateMachine stm = cat.GetComponent<CatStateMachine>();
        stm.Spawner = this;

        strayCatCount++;
        catsInSceneCount++;

        stm.OnStateChanged += (prev, current) => {
            if (prev == stm.STATE_STRAYIDLE || prev == stm.STATE_STRAYWANDER) {
                strayCatCount--;
            }

            if (current == stm.STATE_STRAYIDLE || current == stm.STATE_STRAYWANDER) {
                strayCatCount++;
            }
        };
    }

    public void Return(GameObject cat) {
        catsInSceneCount--;
        if (cat.TryGetComponent(out CatStateMachine stm)) {
            if (stm.CurrentState == stm.STATE_STRAYIDLE || stm.CurrentState == stm.STATE_STRAYWANDER) {
                strayCatCount--;
            }
        }
        strayCatPool.ReturnObject(cat);
    }

    private Vector3 GetSpawnPosition() {
        for (int i = 0; i < maxLocationSearchAttempts; i++) {
            Vector3 spawnPosition = transform.position;
            Vector3 spawnSize = spawnArea.size;

            spawnPosition.x += UnityEngine.Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
            spawnPosition.z += UnityEngine.Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas)) {
                Vector3 hitPosition = hit.position;
                if (hit.position.y > 0.3f) continue;

                // Debug.DrawRay(hitPosition, Vector3.up * 100f, Color.green, 100f);

                // CASE 0: It is inside a building
                if (Physics.OverlapSphere(hitPosition, 0.1f, obstacleMask).Length > 0) continue;

                // CASE 1: Spawn position is obstructed by something (i.e. building)
                Vector3 directionToHit = hitPosition - Camera.main.transform.position;
                float distance = directionToHit.magnitude;

                if (Physics.Raycast(Camera.main.transform.position, directionToHit, distance, obstacleMask))
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
