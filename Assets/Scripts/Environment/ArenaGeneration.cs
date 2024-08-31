using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ArenaGeneration : MonoBehaviour {
    [Header("Editor")]
    #if UNITY_EDITOR
    [SerializeField] private bool showGizmos = true;
    #endif

    [Header("Settings")]
    [InfoBox("Grid Coordinate starts from top left corner at (0, 0)")]
    [SerializeField] private float gridSize = 10f;
    [SerializeField] private Vector2Int arenaGridDimension = new(8, 5);
    [SerializeField] private Arena[] arenaPrefabs;
    [SerializeField] private int maxGenerationAttempt = 20;
    
    // Notes for Grid Coordinate
    // Grid Coordinate starts from top left corner at (0, 0)
    private bool[,] isGridOccupied;
    private GameObject[,] objectsInGrid;
    private int[] remainingGenerationSlot;
    private bool[] arenaMustAppear;

    private void Start() {
        GenerateArena();
    }

    [Button("Generate Random Arena")]
    private void GenerateArena() {
        ClearArena();

        // INITIALIZATION
        isGridOccupied = new bool[arenaGridDimension.x, arenaGridDimension.y];
        objectsInGrid = new GameObject[arenaGridDimension.x, arenaGridDimension.y];

        remainingGenerationSlot = new int[arenaPrefabs.Length];
        for (int i = 0; i < arenaPrefabs.Length; i++) {
            remainingGenerationSlot[i] = arenaPrefabs[i].GetComponent<Arena>().GenerationSlot;
        }

        arenaMustAppear = new bool[arenaPrefabs.Length];
        for (int i = 0; i < arenaPrefabs.Length; i++) {
            arenaMustAppear[i] = arenaPrefabs[i].MustAppear;
        }

        // STEP 1: RANDOMLY GENERATE THE ARENA
        for (int z = 0; z < arenaGridDimension.y; z++) {
            for (int x = 0; x < arenaGridDimension.x; x++) {
                // Skip if the grid is already occupied
                if (isGridOccupied[x, z]) continue;

                Vector2Int coord = new(x, z);

                int prefabIndex = GetRandomArenaPrefabIndex();
                if (prefabIndex == -1) return; // No more arena to be placed

                Arena arena = arenaPrefabs[prefabIndex];
                int attempt = 0;

                // Find the suitable arena that can be placed
                while (!CanBePlaced(coord, arena.GridSpan) && attempt < maxGenerationAttempt) {
                    prefabIndex = GetRandomArenaPrefabIndex();
                    if (prefabIndex == -1) return;
                    arena = arenaPrefabs[prefabIndex];
                    attempt++;
                }
                if (attempt >= maxGenerationAttempt) {
                    Debug.LogWarning("Cannot place the arena at " + coord);
                    return;
                }

                // Spawn the arena if it can be placed
                SpawnArenaAt(coord, arena);
                for (int i = 0; i < arena.GridSpan.x; i++) {
                    for (int j = 0; j < arena.GridSpan.y; j++) {
                        if (x + i >= arenaGridDimension.x || z - j < 0) return;
                        isGridOccupied[x + i, z - j] = true;
                    }
                }

                // Update our counter
                int arenaIndex = System.Array.IndexOf(arenaPrefabs, arena);
                remainingGenerationSlot[arenaIndex]--;
                arenaMustAppear[arenaIndex] = false;
            }
        }

        // STEP 2: IF THE MUST APPEAR ARENA IS NOT PLACED
        // TRY TO REPLACE THE EXISTING ARENA WITH THE MUST APPEAR ARENA
        for (int i = 0; i < arenaPrefabs.Length; i++) {
            if (!arenaMustAppear[i]) continue;

            Arena mustAppearArena = arenaPrefabs[i];

            // Find the replacable arena
            Vector2Int coord = GetRandomGridCoordinate();
            int attempt = 0;
            while (!CanBePlaced(coord, mustAppearArena.GridSpan) && attempt < maxGenerationAttempt) {
                coord = GetRandomGridCoordinate();
                attempt++;
            }

            // Replace the arena
            if (attempt < maxGenerationAttempt) {
                RemoveArenaAt(coord, mustAppearArena);
                SpawnArenaAt(coord, mustAppearArena);
                for (int x = 0; x < mustAppearArena.GridSpan.x; x++) {
                    for (int z = 0; z < mustAppearArena.GridSpan.y; z++) {
                        if (coord.x + x >= arenaGridDimension.x || coord.y - z < 0) return;
                        isGridOccupied[coord.x + x, coord.y - z] = true;
                    }
                }
            } else {
                Debug.LogWarning("Cannot place the must appear arena: " + mustAppearArena.name);
            }
        }
    }

    private bool CanBePlaced(Vector2Int coord, Vector2Int gridSpan) {
        for (int x = 0; x < gridSpan.x; x++) {
            for (int z = 0; z < gridSpan.y; z++) {
                if (coord.x + x >= arenaGridDimension.x || coord.y - z < 0) {
                    return false;
                }
            }
        }

        return true;
    }

    private Vector2Int GetRandomGridCoordinate() {
        int x = Random.Range(0, arenaGridDimension.x);
        int z = Random.Range(0, arenaGridDimension.y);
        return new(x, z);
    }

    private void RemoveArenaAt(Vector2Int coord, Arena arena) {
        for (int i = 0; i < arena.GridSpan.x; i++) {
            for (int j = 0; j < arena.GridSpan.y; j++) {
                if (coord.x + i >= arenaGridDimension.x || coord.y - j < 0) return;

                GameObject obj = objectsInGrid[coord.x + i, coord.y - j];
                if (obj != null) { Destroy(obj); }

                isGridOccupied[coord.x + i, coord.y - j] = false;
            }
        }
    }

    private void SpawnArenaAt(Vector2Int coord, Arena arena) {
        // Calculate the position
        Vector3 position = GridToWorldCoordinate(coord);

        float halfGridSize = gridSize / 2f;
        float offsetFactorX = arena.GridSpan.x - 1f;
        float offsetFactorZ = arena.GridSpan.y - 1f;
        Vector3 originOffset = new(offsetFactorX * halfGridSize, 0f, -offsetFactorZ * halfGridSize);

        position += transform.position;
        position += originOffset;

        // Put the arena
        GameObject arenaObj = Instantiate(arena.gameObject, position, Quaternion.identity);
        arenaObj.GetComponent<Arena>().Init(coord);
        arenaObj.transform.SetParent(transform);

        objectsInGrid[coord.x, coord.y] = arenaObj;
    }

    private Vector3 GridToWorldCoordinate(Vector2Int gridCoord) {
        Vector3 topLeftCorner = new(
            -arenaGridDimension.x * gridSize / 2f + gridSize / 2f,
            0f,
            arenaGridDimension.y * gridSize / 2f - gridSize / 2f
        );
        Vector3 gridOffset = new(gridCoord.x * gridSize, 0f, -gridCoord.y * gridSize);
        Vector3 position = topLeftCorner + gridOffset;

        return position;
    }

    private int GetRandomArenaPrefabIndex() {
        List<int> availablePrefabIndex = new();
        for (int i = 0; i < arenaPrefabs.Length; i++) {
            for (int j = 0; j < remainingGenerationSlot[i]; j++) {
                availablePrefabIndex.Add(i);
            }
        }

        if (availablePrefabIndex.Count == 0) return -1;
        int randomIndex = Random.Range(0, availablePrefabIndex.Count);
        return availablePrefabIndex[randomIndex];
    }

    #if UNITY_EDITOR
    private void OnValidate() {
        if (arenaGridDimension.x < 1) arenaGridDimension.x = 1;
        if (arenaGridDimension.y < 1) arenaGridDimension.y = 1;

        if (gridSize < 1) gridSize = 1;
    }

    [Button("Clear Arena")]
    private void ClearArena() {
        if (Application.isPlaying) {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        } else {
            foreach (Transform child in transform) {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private void OnDrawGizmos() {
        if (!showGizmos) return;
        Gizmos.color = Color.green;

        float startX = -arenaGridDimension.x / 2f;
        float endX = arenaGridDimension.x / 2f;

        float startZ = -arenaGridDimension.y / 2f;
        float endZ = arenaGridDimension.y / 2f;

        for (float x = startX; x < endX; x++) {
            for (float z = startZ; z < endZ; z++) {
                Vector3 position = new(x * gridSize + gridSize / 2, 0, z * gridSize + gridSize / 2);
                position += transform.position;

                Gizmos.DrawWireCube(position, new Vector3(gridSize, 0, gridSize));
            }
        }
    }
    #endif
}