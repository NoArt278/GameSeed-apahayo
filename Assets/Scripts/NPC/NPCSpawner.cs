using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;
using NaughtyAttributes;

public class NPCSpawner : MonoBehaviour
{
    private static int npcInScene = 0;

    [Header("NPC")]
    [SerializeField] private GameObject[] npcPrefabs;
    [SerializeField] private int maxNPCInScene = 25;

    [Header("Spawn")]
    [SerializeField] private float spawnDelay = 10f;
    [SerializeField] private int bulkSpawnRate = 4;
    [SerializeField] private int maxLocationSearchAttempts = 30;
    [SerializeField] private Transform spawnParent;

    [Header("Others")]
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private Canvas sceneCanvas;

    private BoxCollider spawnArea;
    private LayerMask obstacleMask;
    private Coroutine spawnRoutine;
    [SerializeField, ReadOnly] private ObjectPool npcPool;

    private void Awake() {
        spawnArea = GetComponent<BoxCollider>();
        obstacleMask = LayerMask.GetMask("Obstacle");

        npcInScene = 0;
    }

    private void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    [Button]
    private void SerializeObjectPool() {
        npcPool = new ObjectPool(npcPrefabs, 50, spawnParent);
    }

    [Button]
    private void ClearObjectPool() {
        while (spawnParent.childCount > 0) {
            DestroyImmediate(spawnParent.GetChild(0).gameObject);
        }
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

    private IEnumerator SpawnRoutine() {
        while (true) {
            for (int i = 0; i < bulkSpawnRate; i++) {
                Spawn();
                yield return null;
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
    
    public void Return(GameObject npc){
        npcInScene--;
        npcPool.ReturnObject(npc);
    }

    private void Spawn(){
        if (npcInScene >= maxNPCInScene) return;
        if (!Camera.main) return;

        Vector3 randomPosition = GetRandomPositionOnNavMesh();
        if (randomPosition == Vector3.zero) return;

        // Instantiate NPC
        GameObject npc = npcPool.GetObject();
        npc.transform.SetPositionAndRotation(randomPosition, Quaternion.identity);

        npc.GetComponent<NPCStateMachine>().Initialize(navMeshSurface);
        npc.GetComponent<NPCStateMachine>().Spawner = this;
        npc.GetComponent<HypnotizeUIManager>().SetupHypnoBar(sceneCanvas);

        npcInScene++;
    }

    private Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 spawnPosition = transform.position;
        Vector3 spawnSize = spawnArea.size;

        spawnPosition.x += Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        spawnPosition.z += Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

        for (int i = 0; i < maxLocationSearchAttempts; i++)
        {
            NavMeshHit hit;
            float maxDistance = 5f;
            if (NavMesh.SamplePosition(spawnPosition, out hit, maxDistance, NavMesh.AllAreas))
            {
                if (hit.position.y > 0.5f) continue;
                if (Physics.OverlapSphere(hit.position, 0.1f, obstacleMask).Length > 0) continue;

                Vector3 directionToCamera = Camera.main.transform.position - hit.position;
                float distanceToCamera = directionToCamera.magnitude;

                if (Physics.Raycast(hit.position, directionToCamera, distanceToCamera, obstacleMask))
                {
                    return hit.position;
                }

                return hit.position;
            }
        }

        return Vector3.zero;
    }
}
