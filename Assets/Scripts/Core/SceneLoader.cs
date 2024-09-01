using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadEvents {
    public Action OnSceneLoadStart;
    public Action OnSceneLoadComplete;
}

public class SceneLoader : MonoBehaviour {
    public static SceneLoader Instance;
    public string CurrentSceneName => SceneManager.GetActiveScene().name;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void LoadScene(string sceneName, SceneLoadEvents events = null) {
        StartCoroutine(LoadSceneAsync(sceneName, events));
    }

    public IEnumerator LoadSceneAsync(string sceneName, SceneLoadEvents events = null) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone) {
            if (asyncLoad.progress >= 0.9f) {
                yield return null;

                events?.OnSceneLoadStart?.Invoke();
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        events?.OnSceneLoadComplete?.Invoke();
    }

    public void ToGameplay() {
        LoadScene("Gameplay", new SceneLoadEvents{
            // 
            OnSceneLoadComplete = () => { GameManager.Instance.OnGameplaySceneLoaded(); }
        });
    }

    public void QuitGame() {
        Application.Quit();
    }
}