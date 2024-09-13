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

    // Pointer enter suka miss, tp exit gk jadi ngecil mulu
    // Need fix later, for now like this
    public void CursorHover(Transform obj)
    {
        
        obj.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.5f);
    }

    public void CursorExit(Transform obj)
    {
        obj.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.5f);
    }
}
