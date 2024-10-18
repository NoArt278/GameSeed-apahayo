using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class ArenaGeneration : SingletonMB<ArenaGeneration> {
    #if UNITY_EDITOR
    [System.Serializable]
    public class BorderPrefabs {
        public GameObject Side;
        public GameObject Corner;
        public GameObject Bottom;
    }

    [Header("Editor")]
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private GameObject _floorPrefab;
    [SerializeField] private BorderPrefabs _borderPrefabs;
    #endif
    [SerializeField] private Transform _floorBorderParent;

    [Header("Settings")]
    [InfoBox("Grid Coordinate starts from top left corner at (0, 0)")]
    [SerializeField] private float _gridSize = 10f;
    [SerializeField] private Vector2Int _arenaGridDimension = new(8, 5);
    [SerializeField] private Arena[] _arenaPrefabs;
    [SerializeField] private Arena _museumPrefab;
    [SerializeField] private Transform _arenaPropsParent;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private int _maxGenerationAttempt = 20;

    public Vector3 PlayerSpawnPoint => _playerSpawnPoint.position;
    
    // Notes for Grid Coordinate
    // Grid Coordinate starts from top left corner at (0, 0)
    private bool[,] _isGridOccupied;
    private GameObject[,] _objectsInGrid;
    private int[] _remainingGenerationSlot;
    private bool[] _arenaMustAppear;
    private enum Orientation { Default, Rotated90 }
    private Orientation RandomOrientation => Random.Range(0, 2) == 0 ? Orientation.Default : Orientation.Rotated90;

    public RangeFloat HorizontalBounds() {
        float halfGridSize = _gridSize / 2f;
        float startX = _arenaGridDimension.x * halfGridSize + halfGridSize;
        float endX = _arenaGridDimension.x * halfGridSize - halfGridSize;

        startX += transform.position.x;
        endX += transform.position.x;

        return new(startX, endX);
    }

    public RangeFloat VerticalBounds() {
        float halfGridSize = _gridSize / 2f;
        float startZ = _arenaGridDimension.y * halfGridSize - halfGridSize;
        float endZ = -_arenaGridDimension.y * halfGridSize + halfGridSize;

        startZ += transform.position.z;
        endZ += transform.position.z;

        return new(startZ, endZ);
    }

    public float GroundY => transform.position.y;

    [Button("[ARENA] Generate Random Props")]
    public void GenerateArena() {
        ClearArenaProps();

        // INITIALIZATION
        _isGridOccupied = new bool[_arenaGridDimension.x, _arenaGridDimension.y];
        _objectsInGrid = new GameObject[_arenaGridDimension.x, _arenaGridDimension.y];

        _remainingGenerationSlot = new int[_arenaPrefabs.Length];
        for (int i = 0; i < _arenaPrefabs.Length; i++) {
            _remainingGenerationSlot[i] = _arenaPrefabs[i].GetComponent<Arena>().GenerationSlot;
        }

        _arenaMustAppear = new bool[_arenaPrefabs.Length];
        for (int i = 0; i < _arenaPrefabs.Length; i++) {
            _arenaMustAppear[i] = _arenaPrefabs[i].MustAppear;
        }

        PlaceMuseum();

        // STEP 1: RANDOMLY GENERATE THE ARENA
        for (int z = 0; z < _arenaGridDimension.y; z++) {
            for (int x = 0; x < _arenaGridDimension.x; x++) {
                // Skip if the g_rid is already occupied
                if (_isGridOccupied[x, z]) continue;

                Vector2Int coord = new(x, z);

                int prefabIndex = GetRandomArenaPrefabIndex();
                if (prefabIndex == -1) return; // No more arena to be placed

                Arena arena = _arenaPrefabs[prefabIndex];
                Orientation orientation = RandomOrientation;
                int attempt = 0;

                // Find the suitable arena that can be placed
                while (!CanBePlaced(coord, arena.GridSpan, orientation) && attempt < _maxGenerationAttempt) {
                    prefabIndex = GetRandomArenaPrefabIndex();
                    if (prefabIndex == -1) return;
                    arena = _arenaPrefabs[prefabIndex];
                    orientation = RandomOrientation;
                    attempt++;
                }
                if (attempt >= _maxGenerationAttempt) {
                    Debug.LogWarning("Cannot place the arena at " + coord);
                    return;
                }

                // Spawn the arena if it can be placed
                SpawnArenaAt(coord, arena, orientation);
                float spanX = orientation == Orientation.Default ? arena.GridSpan.x : arena.GridSpan.y;
                float spanZ = orientation == Orientation.Default ? arena.GridSpan.y : arena.GridSpan.x;

                for (int i = 0; i < spanX; i++) {
                    for (int j = 0; j < spanZ; j++) {
                        if (x + i >= _arenaGridDimension.x || z + j >= _arenaGridDimension.y) return;
                        _isGridOccupied[x + i, z + j] = true;
                    }
                }

                // Update our counter
                int arenaIndex = System.Array.IndexOf(_arenaPrefabs, arena);
                _remainingGenerationSlot[arenaIndex]--;
                _arenaMustAppear[arenaIndex] = false;
            }
        }

        // STEP 2: IF THE MUST APPEAR ARENA IS NOT PLACED
        // TRY TO REPLACE THE EXISTING ARENA WITH THE MUST APPEAR ARENA
        for (int i = 0; i < _arenaPrefabs.Length; i++) {
            if (!_arenaMustAppear[i]) continue;

            Arena mustAppearArena = _arenaPrefabs[i];

            // Find the replacable arena
            Vector2Int coord = GetRandomGridCoordinate();
            Orientation orientation = RandomOrientation;
            int attempt = 0;
            while (!CanBePlaced(coord, mustAppearArena.GridSpan, orientation) && attempt < _maxGenerationAttempt) {
                coord = GetRandomGridCoordinate();
                orientation = RandomOrientation;
                attempt++;
            }

            // Replace the arena
            if (attempt < _maxGenerationAttempt) {
                RemoveArenaAt(coord, mustAppearArena, orientation);
                SpawnArenaAt(coord, mustAppearArena, orientation);

                float spanX = orientation == Orientation.Default ? mustAppearArena.GridSpan.x : mustAppearArena.GridSpan.y;
                float spanZ = orientation == Orientation.Default ? mustAppearArena.GridSpan.y : mustAppearArena.GridSpan.x;

                for (int x = 0; x < spanX; x++) {
                    for (int z = 0; z < spanZ; z++) {
                        if (coord.x + x >= _arenaGridDimension.x || coord.y + z >= _arenaGridDimension.y) return;
                        _isGridOccupied[coord.x + x, coord.y + z] = true;
                    }
                }
            } else {
                Debug.LogWarning("Cannot place the must appear arena: " + mustAppearArena.name);
            }
        }
    }

    public void PlaceMuseum() {
        Vector2Int coord = new(1, 1);
        SpawnArenaAt(coord, _museumPrefab, faceCamera: true);

        for (int i = 0; i < _museumPrefab.GridSpan.x; i++) {
            for (int j = 0; j < _museumPrefab.GridSpan.y; j++) {
                if (coord.x + i >= _arenaGridDimension.x || coord.y + j >= _arenaGridDimension.y) return;
                _isGridOccupied[coord.x + i, coord.y + j] = true;
            }
        }
    }

    private bool CanBePlaced(Vector2Int coord, Vector2Int gridSpan, Orientation orientation = Orientation.Default) {
        int spanX = orientation == Orientation.Default ? gridSpan.x : gridSpan.y;
        int spanZ = orientation == Orientation.Default ? gridSpan.y : gridSpan.x;

        for (int x = 0; x < spanX; x++) {
            for (int z = 0; z < spanZ; z++) {
                if (coord.x + x >= _arenaGridDimension.x || coord.y + z >= _arenaGridDimension.y) {
                    return false;
                }
            }
        }

        return true;
    }

    private Vector2Int GetRandomGridCoordinate() {
        int x = Random.Range(0, _arenaGridDimension.x);
        int z = Random.Range(0, _arenaGridDimension.y);
        return new(x, z);
    }

    private void RemoveArenaAt(Vector2Int coord, Arena arena, Orientation orientation = Orientation.Default) {
        int spanX = orientation == Orientation.Default ? arena.GridSpan.x : arena.GridSpan.y;
        int spanZ = orientation == Orientation.Default ? arena.GridSpan.y : arena.GridSpan.x;

        for (int i = 0; i < spanX; i++) {
            for (int j = 0; j < spanZ; j++) {
                if (coord.x + i >= _arenaGridDimension.x || coord.y + j >= _arenaGridDimension.y) return;

                GameObject obj = _objectsInGrid[coord.x + i, coord.y + j];
                if (obj != null) { 
                    if (Application.isPlaying) Destroy(obj);
                    else DestroyImmediate(obj);
                }

                _isGridOccupied[coord.x + i, coord.y + j] = false;
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

        float halfGridSize = _gridSize / 2f;
        float offsetFactorX = spanX - 1f;
        float offsetFactorZ = spanZ - 1f;
        Vector3 originOffset = new(offsetFactorX * halfGridSize, 0f, -offsetFactorZ * halfGridSize);

        position += transform.position;
        position += originOffset;

        Quaternion rotation = faceCamera ? Quaternion.Euler(0, 180, 0) : GetRandom180Rotation(orientation);

        // Put the arena
        GameObject arenaObj = Instantiate(arena.gameObject, position, rotation);
        arenaObj.GetComponent<Arena>().Init(coord);
        arenaObj.transform.SetParent(_arenaPropsParent);

        _objectsInGrid[coord.x, coord.y] = arenaObj;
    }

    private Vector3 GridToWorldCoordinate(Vector2Int gridCoord) {
        Vector3 topLeftCorner = new(
            - _arenaGridDimension.x * _gridSize / 2f + _gridSize / 2f,
            0f,
            _arenaGridDimension.y * _gridSize / 2f - _gridSize / 2f
        );
        Vector3 gridOffset = new(gridCoord.x * _gridSize, 0f, -gridCoord.y * _gridSize);
        Vector3 position = topLeftCorner + gridOffset;

        return position;
    }

    private int GetRandomArenaPrefabIndex() {
        List<int> availablePrefabIndex = new();
        for (int i = 0; i < _arenaPrefabs.Length; i++) {
            for (int j = 0; j < _remainingGenerationSlot[i]; j++) {
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
            foreach (Transform child in _arenaPropsParent) {
                Destroy(child.gameObject);
            }
        } else {
            while (_arenaPropsParent.childCount > 0) {
                DestroyImmediate(_arenaPropsParent.GetChild(0).gameObject);
            }
        }
    }

    #if UNITY_EDITOR

    [Button("[FB] Refresh Floor and Border")]
    private void PlaceFloorAndBorder() {
        ClearFloorAndBorder();
        for (int z = 0; z < _arenaGridDimension.y; z++) {
            for (int x = 0; x < _arenaGridDimension.x; x++) {
                Vector2Int coord = new(x, z);
                Vector3 position = GridToWorldCoordinate(coord);
                position += transform.position;

                // Place the floor
                GameObject floor = Instantiate(_floorPrefab, position, Quaternion.identity);
                floor.transform.SetParent(_floorBorderParent);
            }
        }

        for (int z = -1; z < _arenaGridDimension.y + 1; z++) {
            Vector2Int coord = new(-1, z);
            Vector3 position = GridToWorldCoordinate(coord);
            position += transform.position;

            // Place the border
            if (z == -1) {
                GameObject border = Instantiate(_borderPrefabs.Corner, position, Quaternion.Euler(0, 180, 0));
                border.transform.SetParent(_floorBorderParent);
            } else if (z == _arenaGridDimension.y) {
                GameObject border = Instantiate(_borderPrefabs.Corner, position, Quaternion.Euler(0, 90, 0));
                border.transform.SetParent(_floorBorderParent);
            } else {
                GameObject border = Instantiate(_borderPrefabs.Side, position, Quaternion.Euler(0, 90, 0));
                border.transform.SetParent(_floorBorderParent);
            }

            coord = new(_arenaGridDimension.x, z);
            position = GridToWorldCoordinate(coord);
            position += transform.position;

            // Place the border
            if (z == -1) {
                GameObject border = Instantiate(_borderPrefabs.Corner, position, Quaternion.Euler(0, 270, 0));
                border.transform.SetParent(_floorBorderParent);
            } else if (z == _arenaGridDimension.y) {
                GameObject border = Instantiate(_borderPrefabs.Corner, position, Quaternion.Euler(0, 0, 0));
                border.transform.SetParent(_floorBorderParent);
            } else {
                GameObject border = Instantiate(_borderPrefabs.Side, position, Quaternion.Euler(0, 270, 0));
                border.transform.SetParent(_floorBorderParent);
            }
        }

        for (int x = 0; x < _arenaGridDimension.x; x++) {
            Vector2Int coord = new(x, -1);
            Vector3 position = GridToWorldCoordinate(coord);
            position += transform.position;

            // Place the border
            GameObject border = Instantiate(_borderPrefabs.Side, position, Quaternion.Euler(0, 180, 0));
            border.transform.SetParent(_floorBorderParent);

            coord = new(x, _arenaGridDimension.y);
            position = GridToWorldCoordinate(coord);
            position += transform.position;

            // Place the border
            GameObject border2 = Instantiate(_borderPrefabs.Bottom, position, Quaternion.Euler(0, 180, 0));
            border2.transform.SetParent(_floorBorderParent);
        }
    }

    private void OnValidate() {
        if (_arenaGridDimension.x < 1) _arenaGridDimension.x = 1;
        if (_arenaGridDimension.y < 1) _arenaGridDimension.y = 1;

        if (_gridSize < 1) _gridSize = 1;
    }

    private void ClearFloorAndBorder() {
        if (Application.isPlaying) {
            foreach (Transform child in _floorBorderParent) {
                Destroy(child.gameObject);
            }
        } else {
            while (_floorBorderParent.childCount > 0) {
                DestroyImmediate(_floorBorderParent.GetChild(0).gameObject);
            }
        }
    }

    private void OnDrawGizmos() {
        if (!_showGizmos) return;
        Gizmos.color = Color.green;

        float startX = - _arenaGridDimension.x / 2f;
        float endX = _arenaGridDimension.x / 2f;

        float startZ = - _arenaGridDimension.y / 2f;
        float endZ = _arenaGridDimension.y / 2f;

        for (float x = startX; x < endX; x++) {
            for (float z = startZ; z < endZ; z++) {
                Vector3 position = new(x * _gridSize + _gridSize / 2, 0, z * _gridSize + _gridSize / 2);
                position += transform.position;

                Gizmos.DrawWireCube(position, new Vector3(_gridSize, 0, _gridSize));
            }
        }
    }
    #endif
}