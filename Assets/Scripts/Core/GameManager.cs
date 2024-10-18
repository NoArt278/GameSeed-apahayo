using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

[Serializable]
public enum GameState { PreGame, InGame, Paused, PostGame }

public class GameManager : SingletonMB<GameManager>
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _cameraPrefab;
    [SerializeField] private GameObject _directionalLight;
    public GameState CurrentState { get; private set; } = GameState.PreGame;
    public Action<GameState, GameState> OnGameStateChanged;

    public void SetGameState(GameState state)
    {
        if (CurrentState == state) return;
        GameState prev = CurrentState;
        CurrentState = state;

        OnGameStateChanged?.Invoke(prev, CurrentState);
    }

    private void Start() {
        if (SceneLoader.Instance.CurrentSceneName == "Gameplay") OnGameplaySceneLoaded();
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
            DOVirtual.DelayedCall(1.8f, () => InGameScreen.Instance.ShowEndGameScreen());
        };

        AudioManager.Instance.Play("InGameBGM");
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
        GameObject player = Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        GameObject camera = Instantiate(_cameraPrefab);
        CinemachineVirtualCamera vcam = camera.GetComponentInChildren<CinemachineVirtualCamera>();

        CinemachineTargetGroup targetGroup = player.GetComponentInChildren<CinemachineTargetGroup>();
        vcam.Follow = targetGroup.transform;
        vcam.LookAt = targetGroup.transform;
        playerMovement.virtualCamera = vcam;

        WorldPosCrosshair crosshair = player.GetComponentInChildren<WorldPosCrosshair>();
        crosshair.Initialize(camera.GetComponent<Camera>());
    }
}