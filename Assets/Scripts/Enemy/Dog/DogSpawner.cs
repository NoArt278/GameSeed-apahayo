using UnityEngine;
using UnityEngine.AI;

public class DogSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _dogPrefab;
    private GameObject _player;
    [SerializeField] private int _spawnAmount = 2;
    private int _dogsSpawned = 0;

    [Header("Spawn")]
    [SerializeField] private float _minDistanceFromPlayer;
    [SerializeField] private int _maxLocationSearchAttempts = 30;

    private BoxCollider _spawnArea;
    private LayerMask _obstacleMask;

    private void Awake() {
        _obstacleMask = LayerMask.GetMask("Obstacle");
    }

    private void Start()
    {
        _spawnArea = GetComponent<BoxCollider>();
        _player = GameObject.FindGameObjectWithTag("Player");
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable() {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState prev, GameState current)
    {
        if (current == GameState.InGame) {
            while (_dogsSpawned < _spawnAmount) { Spawn(); }
        }
    }

    private void Spawn()
    {
        if (!Camera.main) return;

        Vector3 spawnPosition = GetSpawnPosition();
        if (spawnPosition == Vector3.zero) return;

        Instantiate(_dogPrefab, spawnPosition, Quaternion.identity);
        _dogsSpawned++;
    }

    private Vector3 GetSpawnPosition()
    {
        for (int i = 0; i < _maxLocationSearchAttempts; i++)
        {
            Vector3 spawnPosition = transform.position;
            Vector3 spawnSize = _spawnArea.size;

            spawnPosition.x += Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
            spawnPosition.z += Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 10, 1))
            {
                // Check if it is visible by player or not
                if (hit.position.y > 0.5f) continue;
                Vector3 hitPosition = hit.position;

                if (_player == null) {
                    _player = GameObject.FindGameObjectWithTag("Player");
                }

                if (Vector3.Distance(hitPosition, _player.transform.position) >= _minDistanceFromPlayer)
                {
                    if (Physics.OverlapSphere(hit.position, 0.1f, _obstacleMask).Length > 0) continue;
                    return hitPosition;
                }
            }
        }

        return Vector3.zero;
    }
}
