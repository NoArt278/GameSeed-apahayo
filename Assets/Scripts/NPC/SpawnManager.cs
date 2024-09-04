using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] public NavMeshSurface navMeshSurface;
    [SerializeField] public Canvas sceneCanvas;
    public int maxTry = 20;
    private BoxCollider spawnArea;
    public GameObject prefab;

    public int spawnAmount = 10;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForNavMeshBuild());
        spawnArea = GetComponent<BoxCollider>();
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
                GameObject npc = Instantiate(prefab, randomPosition, Quaternion.identity);
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
        // Calculate the bounds of the collider
        Bounds bounds = spawnArea.bounds;

        // Generate a random position within the bounds
        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        for (int i = 0; i < maxTry; i++)
        {
            NavMeshHit hit;
            float maxDistance = Mathf.Sqrt(Mathf.Pow(bounds.size.x, 2) + Mathf.Pow(bounds.size.z, 2));
            if (NavMesh.SamplePosition(randomPosition, out hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning("Failed to find a valid NavMesh position near the random position.");
        return Vector3.zero;
    }
}
