using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] public NavMeshSurface navMeshSurface;
    public GameObject prefab;

    public int spawnAmount = 10;
    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomPosition = GetRandomPositionOnNavMesh();
            if (randomPosition != Vector3.zero)
            {
                Instantiate(prefab, randomPosition, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(navMeshSurface.transform.position.x - navMeshSurface.size.x / 2, navMeshSurface.transform.position.x + navMeshSurface.size.x / 2),
            navMeshSurface.transform.position.y,
            Random.Range(navMeshSurface.transform.position.z - navMeshSurface.size.z / 2, navMeshSurface.transform.position.z + navMeshSurface.size.z / 2)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero; // Fallback in case no valid position is found
    }
}
