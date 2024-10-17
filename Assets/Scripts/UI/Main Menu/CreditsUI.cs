using UnityEngine;

public class Credits : MonoBehaviour
{
    public void OpenCredits()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        AudioManager.Instance.PlayOneShot("Click");
    }

    public void BackToPrev()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        AudioManager.Instance.PlayOneShot("Click");
    }
}
