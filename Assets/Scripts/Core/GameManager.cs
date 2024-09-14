using System;
using Cinemachine;
using UnityEngine;

[Serializable]
public enum GameState { PreGame, InGame, Paused, PostGame }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private GameObject directionalLight;
    public GameState CurrentState { get; private set; } = GameState.PreGame;
    public Action<GameState, GameState> OnGameStateChanged;

    public void SetGameState(GameState state)
    {
        if (CurrentState == state) return;
        GameState prev = CurrentState;
        CurrentState = state;

        OnGameStateChanged?.Invoke(prev, CurrentState);
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        if (SceneLoader.Instance.CurrentSceneName != "MainMenu") OnGameplaySceneLoaded();
    }

    public void OnGameplaySceneLoaded()
    {
        SetGameState(GameState.PreGame);
        ArenaGeneration.Instance.GenerateArena();
        NavMeshManager.Instance.BuildNavMesh();
        StaticBatchingUtility.Combine(ArenaGeneration.Instance.gameObject);

        InitializePlayer();

        // Timer
        GameTimer.Instance.SetDuration(120f);
        GameTimer.Instance.OnTimeUp += () => {
            InGameScreen.Instance.ShowEndGameScreen();
        };

        GameplayUI.Instance.GameTransitionIn(OnGameTransitionInComplete);
    }

    private void OnGameTransitionInComplete()
    {
        GameTimer.Instance.StartTimer();
        SetGameState(GameState.InGame);
        InGameScreen.Instance.Initialize();
    }

    private void InitializePlayer() {
        Vector3 spawnPosition = ArenaGeneration.Instance.PlayerSpawnPoint;
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        GameObject camera = Instantiate(cameraPrefab);
        CinemachineVirtualCamera vcam = camera.GetComponentInChildren<CinemachineVirtualCamera>();

        CinemachineTargetGroup targetGroup = player.GetComponentInChildren<CinemachineTargetGroup>();
        vcam.Follow = targetGroup.transform;
        vcam.LookAt = targetGroup.transform;
        playerMovement.virtualCamera = vcam;

        WorldPosCrosshair crosshair = player.GetComponentInChildren<WorldPosCrosshair>();
        crosshair.Initialize(camera.GetComponent<Camera>());
    }

    // private Vector3 GetValidSpawnPosition()
    // {
    //     for (int i = 0; i < 100; i++)
    //     {
    //         float x = ArenaGeneration.Instance.HorizontalBounds().RandomValue();
    //         float y = ArenaGeneration.Instance.GroundY;
    //         float z = ArenaGeneration.Instance.VerticalBounds().RandomValue();

    //         if (NavMesh.SamplePosition(new Vector3(x, y, z), out NavMeshHit hit, 100f, NavMesh.AllAreas))
    //         {
    //             if (Physics.OverlapSphere(hit.position, 0.1f, obstacleMask).Length > 0) continue;
    //             return hit.position;
    //         } 
    //     }

    //     return Vector3.zero;
    // }
}