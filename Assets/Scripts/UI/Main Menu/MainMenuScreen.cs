using DG.Tweening;
using UnityEngine;

public class MainMenuScreen : MonoBehaviour
{
    // Pointer enter suka miss, tp exit gk jadi ngecil mulu
    // Need fix later, for now like this
    public void CursorHover(Transform obj)
    {
        obj.DOScale(new Vector3(1.43f, 1.43f, 1.43f), 0.1f).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Hover");
    }

    public void CursorHoverV2(Transform obj)
    {
        obj.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetUpdate(true);
        AudioManager.Instance.PlayOneShot("Hover");
    }

    public void CursorExit(Transform obj)
    {
        obj.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.1f).SetUpdate(true);
    }

    public void CursorExitV2(Transform obj)
    {
        obj.DOScale(new Vector3(1, 1, 1), 0.1f).SetUpdate(true);
    }
}
