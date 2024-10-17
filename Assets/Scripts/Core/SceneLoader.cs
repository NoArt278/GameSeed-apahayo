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
    [SerializeField] private Image overlay;
    private Action onLoaderCallback;
    public AsyncOperation asyncLoad;

    public void LoadScene(string sceneName, SceneLoadEvents events = null) {
        Time.timeScale = 1;
        DOTween.KillAll();
        AudioManager.Instance.StopBGMFadeOut(1f);
        overlay.DOFade(1, 0.5f).OnComplete(() => {
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

                events?.BeforeAwake?.Invoke();
            }

            yield return null;
        }

        events?.AfterAwake?.Invoke();
    }

    public void AllowSceneActivation() {
        asyncLoad.allowSceneActivation = true;
        overlay.DOFade(1, 0f);
        overlay.DOFade(0, 0.2f);
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
        if (Instance.onLoaderCallback != null)
        {
            Instance.onLoaderCallback();
            Instance.onLoaderCallback = null;
        }
    }
}