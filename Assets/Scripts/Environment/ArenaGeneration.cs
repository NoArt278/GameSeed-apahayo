using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class ArenaGeneration : MonoBehaviour {
    public static ArenaGeneration Instance;

    #if UNITY_EDITOR
    [System.Serializable]
    public class BorderPrefabs {
        public GameObject bottom;
        public GameObject side;
        public GameObject corner;
    }

    [Header("Editor")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private BorderPrefabs borderPrefabs;
    #endif
    [SerializeField] private Transform floorBorderParent;

    [Header("Settings")]
    [InfoBox("Grid Coordinate starts from top left corner at (0, 0)")]
    [SerializeField] private float gridSize = 10f;
    [SerializeField] private Vector2Int arenaGridDimension = new(8, 5);
    [SerializeField] private Arena[] arenaPrefabs;
    [SerializeField] private Arena museumPrefab;
    [SerializeField] private Transform arenaPropsParent;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private int maxGenerationAttempt = 20;

    public Vector3 PlayerSpawnPoint => playerSpawnPoint.position;
    
    // Notes for Grid Coordinate
    // Grid Coordinate starts from top left corner at (0, 0)
    private bool[,] isGridOccupied;
    private GameObject[,] objectsInGrid;
    private int[] remainingGenerationSlot;
    private bool[] arenaMustAppear;
    private enum Orientation { Default, Rotated90 }
    private Orientation RandomOrientation => Random.Range(0, 2) == 0 ? Orientation.Default : Orientation.Rotated90;

    public RangeFloat HorizontalBounds() {
        float halfGridSize = gridSize / 2f;
        float startX = -arenaGridDimension.x * halfGridSize + halfGridSize;
        float endX = arenaGridDimension.x * halfGridSize - halfGridSize;

        startX += transform.position.x;
        endX += transform.position.x;

        return new(startX, endX);
    }

    public RangeFloat VerticalBounds() {
        float halfGridSize = gridSize / 2f;
        float startZ = arenaGridDimension.y * halfGridSize - halfGridSize;
        float endZ = -arenaGridDimension.y * halfGridSize + halfGridSize;

        startZ += transform.position.z;
        endZ += transform.position.z;

        return new(startZ, endZ);
    }

    public float GroundY => transform.position.y;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    [Button("[ARENA] Generate Random Props")]
    public void GenerateArena() {
        ClearArenaProps();

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

        PlaceMuseum();

        // STEP 1: RANDOMLY GENERATE THE ARENA
        for (int z = 0; z < arenaGridDimension.y; z++) {
            for (int x = 0; x < arenaGridDimension.x; x++) {
                // Skip if the grid is already occupied
                if (isGridOccupied[x, z]) continue;

                Vector2Int coord = new(x, z);

                int prefabIndex = GetRandomArenaPrefabIndex();
                if (prefabIndex == -1) return; // No more arena to be placed

                Arena arena = arenaPrefabs[prefabIndex];
                Orientation orientation = RandomOrientation;
                int attempt = 0;

                // Find the suitable arena that can be placed
                while (!CanBePlaced(coord, arena.GridSpan, orientation) && attempt < maxGenerationAttempt) {
                    prefabIndex = GetRandomArenaPrefabIndex();
                    if (prefabIndex == -1) return;
                    arena = arenaPrefabs[prefabIndex];
                    orientation = RandomOrientation;
                    attempt++;
                }
                if (attempt >= maxGenerationAttempt) {
                    Debug.LogWarning("Cannot place the arena at " + coord);
                    return;
                }

                // Spawn the arena if it can be placed
                SpawnArenaAt(coord, arena, orientation);
                float spanX = orientation == Orientation.Default ? arena.GridSpan.x : arena.GridSpan.y;
                float spanZ = orientation == Orientation.Default ? arena.GridSpan.y : arena.GridSpan.x;

                for (int i = 0; i < spanX; i++) {
                    for (int j = 0; j < spanZ; j++) {
                        if (x + i >= arenaGridDimension.x || z + j >= arenaGridDimension.y) return;
                        isGridOccupied[x + i, z + j] = true;
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
            Orientation orientation = RandomOrientation;
            int attempt = 0;
            while (!CanBePlaced(coord, mustAppearArena.GridSpan, orientation) && attempt < maxGenerationAttempt) {
                coord = GetRandomGridCoordinate();
                orientation = RandomOrientation;
                attempt++;
            }

            // Replace the arena
            if (attempt < maxGenerationAttempt) {
                RemoveArenaAt(coord, mustAppearArena, orientation);
                SpawnArenaAt(coord, mustAppearArena, orientation);

                float spanX = orientation == Orientation.Default ? mustAppearArena.GridSpan.x : mustAppearArena.GridSpan.y;
                float spanZ = orientation == Orientation.Default ? mustAppearArena.GridSpan.y : mustAppearArena.GridSpan.x;

                for (int x = 0; x < spanX; x++) {
                    for (int z = 0; z < spanZ; z++) {
                        if (coord.x + x >= arenaGridDimension.x || coord.y + z >= arenaGridDimension.y) return;
                        isGridOccupied[coord.x + x, coord.y + z] = true;
                    }
                }
            } else {
                Debug.LogWarning("Cannot place the must appear arena: " + mustAppearArena.name);
            }
        }
    }

    public void PlaceMuseum() {
        Vector2Int coord = new(1, 1);
        SpawnArenaAt(coord, museumPrefab, faceCamera: true);

        for (int i = 0; i < museumPrefab.GridSpan.x; i++) {
            for (int j = 0; j < museumPrefab.GridSpan.y; j++) {
                if (coord.x + i >= arenaGridDimension.x || coord.y + j >= arenaGridDimension.y) return;
                isGridOccupied[coord.x + i, coord.y + j] = true;
            }
        }
    }

    private bool CanBePlaced(Vector2Int coord, Vector2Int gridSpan, Orientation orientation = Orientation.Default) {
        int spanX = orientation == Orientation.Default ? gridSpan.x : gridSpan.y;
        int spanZ = orientation == Orientation.Default ? gridSpan.y : gridSpan.x;

        for (int x = 0; x < spanX; x++) {
            for (int z = 0; z < spanZ; z++) {
                if (coord.x + x >= arenaGridDimension.x || coord.y + z >= arenaGridDimension.y) {
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

    private void RemoveArenaAt(Vector2Int coord, Arena arena, Orientation orientation = Orientation.Default) {
        int spanX = orientation == Orientation.Default ? arena.GridSpan.x : arena.GridSpan.y;
        int spanZ = orientation == Orientation.Default ? arena.GridSpan.y : arena.GridSpan.x;

        for (int i = 0; i < spanX; i++) {
            for (int j = 0; j < spanZ; j++) {
                if (coord.x + i >= arenaGridDimension.x || coord.y + j >= arenaGridDimension.y) return;

                GameObject obj = objectsInGrid[coord.x + i, coord.y + j];
                if (obj != null) { 
                    if (Application.isPlaying) Destroy(obj);
                    else DestroyImmediate(obj);
                }

                isGridOccupied[coord.x + i, coord.y + j] = false;
            }
        }
    }

    private Quaternion GetRandom180Rotation(Orientation orientation) {
        return orientation == Orientation.Default ? 
            Quaternion.Euler(0, 0 + Random.Range(0, 2) * 180, 0) :
            Quaternion.Euler(0, 90 + Random.Range(0, 2) * 180, 0);
    }

    private void SpawnArenaAt(Vector2Int coord, Arena arena, Orientation orientation = Orientation.Default, bool faceCamera = false) {
        // Calculate the position
        Vector3 position = GridToWorldCoordinate(coord);

        int spanX = orientation == Orientation.Default ? arena.GridSpan.x : arena.GridSpan.y;
        int spanZ = orientation == Orientation.Default ? arena.GridSpan.y : arena.GridSpan.x;

        float halfGridSize = gridSize / 2f;
        float offsetFactorX = spanX - 1f;
        float offsetFactorZ = spanZ - 1f;
        Vector3 originOffset = new(offsetFactorX * halfGridSize, 0f, -offsetFactorZ * halfGridSize);

        position += transform.position;
        position += originOffset;

        Quaternion rotation = faceCamera ? Quaternion.Euler(0, 180, 0) : GetRandom180Rotation(orientation);

        // Put the arena
        GameObject arenaObj = Instantiate(arena.gameObject, position, rotation);
        arenaObj.GetComponent<Arena>().Init(coord);
        arenaObj.transform.SetParent(arenaPropsParent);

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

    [Button("[ARENA] Clear Props")]
    private void ClearArenaProps() {
        if (Application.isPlaying) {
            foreach (Transform child in arenaPropsParent) {
                Destroy(child.gameObject);
            }
        } else {
            while (arenaPropsParent.childCount > 0) {
                DestroyImmediate(arenaPropsParent.GetChild(0).gameObject);
            }
        }
    }

    #if UNITY_EDITOR

    [Button("[FB] Refresh Floor and Border")]
    private void PlaceFloorAndBorder() {
        ClearFloorAndBorder();
        for (int z = 0; z < arenaGridDimension.y; z++) {
            for (int x = 0; x < arenaGridDimension.x; x++) {
                Vector2Int coord = new(x, z);
                Vector3 position = GridToWorldCoordinate(coord);
                position += transform.position;

                // Place the floor
                GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity);
                floor.transform.SetParent(floorBorderParent);
            }
        }

        for (int z = -1; z < arenaGridDimension.y + 1; z++) {
            Vector2Int coord = new(-1, z);
            Vector3 position = GridToWorldCoordinate(coord);
            position += transform.position;

            // Place the border
            if (z == -1) {
                GameObject border = Instantiate(borderPrefabs.corner, position, Quaternion.Euler(0, 180, 0));
                border.transform.SetParent(floorBorderParent);
            } else if (z == arenaGridDimension.y) {
                GameObject border = Instantiate(borderPrefabs.corner, position, Quaternion.Euler(0, 90, 0));
                border.transform.SetParent(floorBorderParent);
            } else {
                GameObject border = Instantiate(borderPrefabs.side, position, Quaternion.Euler(0, 180, 0));
                border.transform.SetParent(floorBorderParent);
            }

            coord = new(arenaGridDimension.x, z);
            position = GridToWorldCoordinate(coord);
            position += transform.position;

            // Place the border
            if (z == -1) {
                GameObject border = Instantiate(borderPrefabs.corner, position, Quaternion.Euler(0, 270, 0));
                border.transform.SetParent(floorBorderParent);
            } else if (z == arenaGridDimension.y) {
                GameObject border = Instantiate(borderPrefabs.corner, position, Quaternion.Euler(0, 0, 0));
                border.transform.SetParent(floorBorderParent);
            } else {
                GameObject border = Instantiate(borderPrefabs.side, position, Quaternion.identity);
                border.transform.SetParent(floorBorderParent);
            }
        }

        for (int x = 0; x < arenaGridDimension.x; x++) {
            Vector2Int coord = new(x, -1);
            Vector3 position = GridToWorldCoordinate(coord);
            position += transform.position;

            // Place the border
            GameObject border = Instantiate(borderPrefabs.bottom, position, Quaternion.Euler(0, 180, 0));
            border.transform.SetParent(floorBorderParent);

            coord = new(x, arenaGridDimension.y);
            position = GridToWorldCoordinate(coord);
            position += transform.position;

            // Place the border
            GameObject border2 = Instantiate(borderPrefabs.bottom, position, Quaternion.Euler(0, 0, 0));
            border2.transform.SetParent(floorBorderParent);
        }
    }

    private void OnValidate() {
        if (arenaGridDimension.x < 1) arenaGridDimension.x = 1;
        if (arenaGridDimension.y < 1) arenaGridDimension.y = 1;

        if (gridSize < 1) gridSize = 1;
    }

    private void ClearFloorAndBorder() {
        if (Application.isPlaying) {
            foreach (Transform child in floorBorderParent) {
                Destroy(child.gameObject);
            }
        } else {
            while (floorBorderParent.childCount > 0) {
                DestroyImmediate(floorBorderParent.GetChild(0).gameObject);
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