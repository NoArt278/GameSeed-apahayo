using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class CatSpawner : MonoBehaviour
{
    private static int strayCatCount = 0;
    private static int catsInSceneCount = 0;

    [SerializeField] private GameObject strayCatPrefab;
    [SerializeField] private int maxStrayCats = 20;
    [SerializeField] private int maxCatsInScene = 25;

    [Header("Spawn")]
    [SerializeField] private float spawnDelay = 10f;
    [SerializeField] private int bulkSpawnRate = 4;
    [SerializeField] private int maxLocationSearchAttempts = 30;

    private LayerMask obstacleMask;
    private BoxCollider spawnArea;
    private Coroutine spawnRoutine;

    private void Awake() {
        spawnArea = GetComponent<BoxCollider>();
        obstacleMask = LayerMask.GetMask("Obstacle");
    }

    private void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
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

    private void Spawn() {
        if (strayCatCount >= maxStrayCats) return;
        if (catsInSceneCount >= maxCatsInScene) return;
        if (!Camera.main) return;

        Vector3 spawnPosition = GetSpawnPosition();
        if (spawnPosition == Vector3.zero) return;

        GameObject strayCat = Instantiate(strayCatPrefab, spawnPosition, Quaternion.identity);
        strayCat.transform.SetParent(transform);
        strayCatCount++;
        catsInSceneCount++;

        strayCat.GetComponent<CatBehaviourManager>().OnStateChanged += (prev, _) => {
            if (prev == CatBehaviourManager.State.Stray) strayCatCount--;
        };

        strayCat.GetComponent<CatBehaviourManager>().OnDestroyed += () => { 
            strayCatCount--; 
            catsInSceneCount--;
        };
    }

    private Vector3 GetSpawnPosition() {
        for (int i = 0; i < maxLocationSearchAttempts; i++) {
            Vector3 spawnPosition = transform.position;
            Vector3 spawnSize = spawnArea.size;

            spawnPosition.x += Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
            spawnPosition.z += Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 10, NavMesh.AllAreas)) {
                Vector3 hitPosition = hit.position;
                if (hit.position.y > 0.5f) continue;

                // CASE 0: It is inside a building
                if (Physics.OverlapSphere(hitPosition, 0.1f, obstacleMask).Length > 0) continue;

                // CASE 1: Spawn position is obstructed by something (i.e. building)
                Vector3 directionToCamera = Camera.main.transform.position - hitPosition;
                float distanceToCamera = directionToCamera.magnitude;

                if (Physics.Raycast(hitPosition, directionToCamera, out RaycastHit raycastHit, distanceToCamera, obstacleMask))
                {
                    if (raycastHit.collider.gameObject != Camera.main.gameObject)
                    {
                        return hitPosition;
                    }
                }

                // CASE 2: It is outside of the camera view
                else
                {
                    Vector2 clipSpace = Camera.main.WorldToViewportPoint(hitPosition);
                    if (clipSpace.x < 0 || clipSpace.x > 1 || clipSpace.y < 0 || clipSpace.y > 1)
                    {
                        return hitPosition;
                    }
                }
            }
        }

        return Vector3.zero;
    }
}
