using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogSpawner : MonoBehaviour
{
    [SerializeField] private GameObject dogPrefab;
    private GameObject Player;
    [SerializeField] private int spawnAmount = 2;
    private int dogsSpawned = 0;

    [Header("Spawn")]
    [SerializeField] private float minDistanceFromPlayer;
    [SerializeField] private int maxLocationSearchAttempts = 30;

    private BoxCollider spawnArea;
    private LayerMask obstacleMask;

    private void Awake() {
        obstacleMask = LayerMask.GetMask("Obstacle");
    }

    private void Start()
    {
        spawnArea = GetComponent<BoxCollider>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if(Player != null)
        {
            if (dogsSpawned < 2)
            {
                Spawn();
            }
        }
        else
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void Spawn()
    {
        if (!Camera.main) return;

        Vector3 spawnPosition = GetSpawnPosition();
        if (spawnPosition == Vector3.zero) return;

        Instantiate(dogPrefab, spawnPosition, Quaternion.identity);
        dogsSpawned++;
    }

    private Vector3 GetSpawnPosition()
    {
        for (int i = 0; i < maxLocationSearchAttempts; i++)
        {
            Vector3 spawnPosition = transform.position;
            Vector3 spawnSize = spawnArea.size;

            spawnPosition.x += Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
            spawnPosition.z += Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 10, 1))
            {
                // Check if it is visible by player or not

                Vector3 hitPosition = hit.position;
                // Vector3 directionToCamera = Camera.main.transform.position - hitPosition;
                // float distanceToCamera = directionToCamera.magnitude;

                if (Vector3.Distance(hitPosition, Player.transform.position) >= minDistanceFromPlayer)
                {
                    if (Physics.OverlapSphere(hit.position, 0.1f, obstacleMask).Length > 0) continue;
                    // bool isInsideBuilding = false;
                    // Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.1f);
                    // foreach(Collider collider in colliders)
                    // {
                    //     if (collider.CompareTag("Building"))
                    //     {
                    //         isInsideBuilding = true;
                    //         break;
                    //     }
                    // }

                    return hitPosition;
                }
            }
        }

        return Vector3.zero;
    }
}
