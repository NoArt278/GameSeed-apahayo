using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] public NavMeshSurface navMeshSurface;
    [SerializeField] public Canvas sceneCanvas;
    public int maxTry = 20;
    private BoxCollider spawnArea;
    public GameObject prefab;

    public int spawnAmount = 10;
    // Start is called before the first frame update

    private void Awake() {
        spawnArea = GetComponent<BoxCollider>();
    }

    void Start()
    {
        StartCoroutine(WaitForNavMeshBuild());
    }

    private IEnumerator WaitForNavMeshBuild()
    {
        // Wait until the NavMeshSurface has been built
        while (navMeshSurface.navMeshData == null)
        {
            Debug.Log("Waiting for NavMeshSurface to be built...");
            yield return new WaitForSeconds(0.5f); // Check every 0.5 seconds
        }

        Debug.Log("NavMeshSurface has been built. Proceeding with spawning.");

        // Execute the spawning logic
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomPosition = GetRandomPositionOnNavMesh();
            if (randomPosition != Vector3.zero)
            {
                GameObject npc = Instantiate(prefab, randomPosition, Quaternion.identity, transform);
                HypnotizeManager hypnotizeManager = npc.GetComponent<HypnotizeManager>();
                if (hypnotizeManager != null)
                {
                    hypnotizeManager.SetupHypnoBar(sceneCanvas);
                }
                else
                {
                    Debug.LogError("HypnotizeManager component not found on the instantiated prefab.");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 spawnPosition = transform.position;
        Vector3 spawnSize = spawnArea.size;

        spawnPosition.x += Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        spawnPosition.z += Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

        for (int i = 0; i < maxTry; i++)
        {
            NavMeshHit hit;
            float maxDistance = 5f;
            if (NavMesh.SamplePosition(spawnPosition, out hit, maxDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning("Failed to find a valid NavMesh position near the random position.");
        return Vector3.zero;
    }
}
