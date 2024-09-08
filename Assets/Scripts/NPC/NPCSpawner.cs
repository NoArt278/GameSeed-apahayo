using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private Canvas sceneCanvas;
    public int maxAttempt = 20;
    private BoxCollider spawnArea;
    public GameObject prefab;

    public int spawnAmount = 10;
    public int npcInScene = 0;

    private bool spawnFlag = false;

    private void Awake() {
        spawnArea = GetComponent<BoxCollider>();
    }

    private void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable() {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }


    private void OnGameStateChanged(GameState prev, GameState current) {
        if (current == GameState.InGame) {
            spawnFlag = true;
        }
    }

    void Update()
    {
        if (!spawnFlag) return;
        if (npcInScene < spawnAmount)
        {
            Spawn();
        }
    }

    private void Spawn(){
        Vector3 randomPosition = GetRandomPositionOnNavMesh();
        if (randomPosition != Vector3.zero)
        {
            GameObject npc = Instantiate(prefab, randomPosition, Quaternion.identity, transform);
            npc.GetComponent<NPCStateMachine>().Initialize(navMeshSurface);

            npc.GetComponent<NPCStateMachine>().spawner = this;

            HypnotizeUIManager hypnotizeManager = npc.GetComponent<HypnotizeUIManager>();
            if (hypnotizeManager != null)
            {
                hypnotizeManager.SetupHypnoBar(sceneCanvas);
            }
            else
            {
                Debug.LogError("HypnotizeManager component not found on the instantiated prefab.");
            }

            npcInScene++;
        }
    }

    private Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 spawnPosition = transform.position;
        Vector3 spawnSize = spawnArea.size;

        spawnPosition.x += Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        spawnPosition.z += Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

        for (int i = 0; i < maxAttempt; i++)
        {
            NavMeshHit hit;
            float maxDistance = 5f;
            if (NavMesh.SamplePosition(spawnPosition, out hit, maxDistance, NavMesh.AllAreas))
            {
                if (hit.position.y > 0.5f) continue;
                return hit.position;
            }
        }

        return Vector3.zero;
    }
}
