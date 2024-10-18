using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;
using NaughtyAttributes;

public class NPCSpawner : MonoBehaviour
{
    private int npcInScene = 0;

    [Header("NPC")]
    [SerializeField] private GameObject[] _npcPrefabs;
    [SerializeField] private int _maxNPCInScene = 25;

    [Header("Spawn")]
    [SerializeField] private float _spawnDelay = 10f;
    [SerializeField] private int _bulkSpawnRate = 4;
    [SerializeField] private int _initialSpawn = 12;
    [SerializeField] private int _maxLocationSearchAttempts = 30;
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private Transform[] _fixedSpawnPoints;

    [Header("Others")]
    [SerializeField] private NavMeshSurface _navMeshSurface;

    private BoxCollider _spawnArea;
    private LayerMask _obstacleMask;
    private Coroutine _spawnRoutine;
    [SerializeField, ReadOnly] private ObjectPool _npcPool;

    private bool _firstSpawn = true;

    private void Awake() {
        _spawnArea = GetComponent<BoxCollider>();
        _obstacleMask = LayerMask.GetMask("Obstacle");
    }

    private void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        npcInScene = 0;
    }

    [Button]
    private void SerializeObjectPool() {
        _npcPool = new ObjectPool(_npcPrefabs, 50, _spawnParent);
    }

    [Button]
    private void ClearObjectPool() {
        while (_spawnParent.childCount > 0) {
            DestroyImmediate(_spawnParent.GetChild(0).gameObject);
        }

        _npcPool = null;
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
            GameObject npc = _npcPool.GetObject();
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();

            agent.Warp(_fixedSpawnPoints[i].position);

            NPCStateMachine stm = npc.GetComponent<NPCStateMachine>();

            stm.Initialize(_navMeshSurface);
            stm.Spawner = this;
            npcInScene++;
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
    
    public void Return(GameObject npc){
        npcInScene--;
        _npcPool.ReturnObject(npc);
    }

    private void Spawn(){
        if (npcInScene >= _maxNPCInScene) return;
        if (!Camera.main) return;

        Vector3 randomPosition = GetRandomPositionOnNavMesh();
        if (randomPosition == Vector3.zero) {
            Debug.LogWarning("Failed to find a valid spawn position");
            return;
        }

        // Instantiate NPC
        GameObject npc = _npcPool.GetObject();
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();

        agent.Warp(randomPosition);

        npc.GetComponent<NPCStateMachine>().Initialize(_navMeshSurface);
        npc.GetComponent<NPCStateMachine>().Spawner = this;

        npcInScene++;
    }

    private Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 spawnPosition = transform.position;
        Vector3 spawnSize = _spawnArea.size;

        spawnPosition.x += Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        spawnPosition.z += Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

        for (int i = 0; i < _maxLocationSearchAttempts; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 10f, NavMesh.AllAreas))
            {
                Vector3 hitPosition = hit.position;
                if (hit.position.y > 3f) continue;

                // CASE 0: It is inside a building
                // if (Physics.OverlapSphere(hit.position, 0.1f, obstacleMask).Length > 0) continue;

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
