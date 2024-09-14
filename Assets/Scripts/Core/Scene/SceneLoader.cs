using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneLoadEvents {
    public Action OnSceneLoadStart;
    public Action OnSceneLoadComplete;
}

public class SceneLoader : MonoBehaviour {
    public static SceneLoader Instance;
    public string CurrentSceneName => SceneManager.GetActiveScene().name;
    [SerializeField] private Image overlay;
    private Action onLoaderCallback;
    public AsyncOperation asyncLoad;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void LoadScene(string sceneName, SceneLoadEvents events = null) {
        Time.timeScale = 1;
        DOTween.KillAll();
        overlay.DOFade(1, 0.2f).OnComplete(() => {
            onLoaderCallback = () => { StartCoroutine(LoadSceneAsync(sceneName, events)); };
            SceneManager.LoadScene("LoadingScreen");
            overlay.DOFade(0, 0f);
        });
    }

    public IEnumerator LoadSceneAsync(string sceneName, SceneLoadEvents events = null) {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone) {
            if (asyncLoad.progress >= 0.9f) {
                yield return null;

                events?.OnSceneLoadStart?.Invoke();
            }

            yield return null;
        }

        events?.OnSceneLoadComplete?.Invoke();
    }

    public void AllowSceneActivation() {
        asyncLoad.allowSceneActivation = true;
        overlay.DOFade(1, 0f);
        overlay.DOFade(0, 0.2f);
    }

    public void ToGameplay() {
        LoadScene("Gameplay", new SceneLoadEvents{
            OnSceneLoadComplete = () => { GameManager.Instance.OnGameplaySceneLoaded(); }
        });
    }

    public void RestartGameplay() {
        ToGameplay();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public static void LoaderCallback()
    {
        if (Instance.onLoaderCallback != null)
        {
            Instance.onLoaderCallback();
            Instance.onLoaderCallback = null;
        }
    }
}