using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneLoadEvents {
    public Action BeforeAwake;
    public Action AfterAwake;
}

public class SceneLoader : SingletonMB<SceneLoader> {
    public string CurrentSceneName => SceneManager.GetActiveScene().name;
    [SerializeField] private Image _overlay;
    private Action _onLoaderCallback;
    public AsyncOperation AsyncLoad;

    public void LoadScene(string sceneName, SceneLoadEvents events = null) {
        Time.timeScale = 1;
        DOTween.KillAll();
        AudioManager.Instance.StopBGMFadeOut(1f);
        _overlay.DOFade(1, 0.5f).OnComplete(() => {
            _onLoaderCallback = () => { StartCoroutine(LoadSceneAsync(sceneName, events)); };
            SceneManager.LoadScene("LoadingScreen");
            _overlay.DOFade(0, 0f);
        });
    }

    public IEnumerator LoadSceneAsync(string sceneName, SceneLoadEvents events = null) {
        AsyncLoad = SceneManager.LoadSceneAsync(sceneName);
        AsyncLoad.allowSceneActivation = false;

        while (!AsyncLoad.isDone) {
            if (AsyncLoad.progress >= 0.9f) {
                yield return null;

                events?.BeforeAwake?.Invoke();
            }

            yield return null;
        }

        events?.AfterAwake?.Invoke();
    }

    public void AllowSceneActivation() {
        if (AsyncLoad == null) {
            Debug.LogWarning("No scene is being loaded");
            return;
        }

        AsyncLoad.allowSceneActivation = true;
        _overlay.DOFade(1, 0f);
        _overlay.DOFade(0, 0.2f);
    }

    public void ToGameplay() {
        LoadScene("Gameplay", new SceneLoadEvents{
            AfterAwake = () => { GameManager.Instance.OnGameplaySceneLoaded(); }
        });
    }

    public void RestartGameplay() {
        AudioManager.Instance.StopBGMFadeOut(1f);
        ToGameplay();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public static void LoaderCallback()
    {
        if (Instance._onLoaderCallback != null)
        {
            Instance._onLoaderCallback();
            Instance._onLoaderCallback = null;
        }
    }
}