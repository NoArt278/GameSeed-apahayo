using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private GameObject directionalLight;
    private LayerMask obstacleMask;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        obstacleMask = LayerMask.GetMask("Obstacle");
    }

    private void Start() {
        if (SceneLoader.Instance.CurrentSceneName != "MainMenu") OnGameplaySceneLoaded();
    }

    public void OnGameplaySceneLoaded()
    {
        ArenaGeneration.Instance.GenerateArena();
        NavMeshManager.Instance.BuildNavMesh();
        InitializePlayer();

        // Timer
        GameTimer.Instance.SetDuration(180f);
        GameTimer.Instance.StartTimer();
        GameTimer.Instance.OnTimeUp += () => {
            EndGameScreen.Instance.ShowEndGameScreen();
        };

        StaticBatchingUtility.Combine(ArenaGeneration.Instance.ArenaPropsParent.gameObject);
        StaticBatchingUtility.Combine(ArenaGeneration.Instance.FloorBorderParent.gameObject);
    }

    private void InitializePlayer() {
        Vector3 spawnPosition = GetValidSpawnPosition();
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        GameObject camera = Instantiate(cameraPrefab);
        CinemachineVirtualCamera vcam = camera.GetComponentInChildren<CinemachineVirtualCamera>();
        vcam.Follow = player.transform;
        vcam.LookAt = player.transform;
        playerMovement.virtualCamera = vcam;
    }

    private Vector3 GetValidSpawnPosition()
    {
        for (int i = 0; i < 100; i++)
        {
            float x = ArenaGeneration.Instance.HorizontalBounds().RandomValue();
            float y = ArenaGeneration.Instance.GroundY;
            float z = ArenaGeneration.Instance.VerticalBounds().RandomValue();

            if (NavMesh.SamplePosition(new Vector3(x, y, z), out NavMeshHit hit, 100f, NavMesh.AllAreas))
            {
                if (Physics.OverlapSphere(hit.position, 0.1f, obstacleMask).Length > 0) continue;

                // bool isInsideBuilding = false;
                // Collider[] colliders = Physics.OverlapSphere(hit.position, 0.1f);
                // foreach (Collider collider in colliders) {
                //     if (collider.CompareTag("Building")) {
                //         isInsideBuilding = true;
                //         break;
                //     }
                // }

                // if (isInsideBuilding) continue;
                return hit.position;
            } 
        }

        return Vector3.zero;
    }

    // private IEnumerator GameplaySetup() {
    //     yield return null;
    // }
}