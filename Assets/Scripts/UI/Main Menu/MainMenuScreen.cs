using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScreen : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoader.Instance.ToGameplay();
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void CursorHover(Transform obj)
    {
        obj.DOScale(obj.localScale * 1.1f, 0.5f);
    }

    public void CursorExit(Transform obj)
    {
        obj.DOScale(obj.localScale / 1.1f, 0.5f);
    }
}
