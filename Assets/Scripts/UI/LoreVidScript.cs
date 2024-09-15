using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class LoreVidScript : MonoBehaviour
{
    [SerializeField] GameObject skipText;
    [SerializeField] VideoPlayer vidPlayer;
    private bool canSkip;

    private void Awake()
    {
        canSkip = PlayerPrefs.GetInt("CanSkipLore", 0) == 1;
        skipText.SetActive(canSkip);
    }

    private void Update()
    {
        if (!vidPlayer.isPlaying && vidPlayer.frame == (long) vidPlayer.frameCount - 1)
        {
            canSkip = true;
            skipText.SetActive(true);
            TMP_Text skipT = skipText.GetComponent<TMP_Text>();
            skipT.text = "Click to continue";
            skipT.color = Color.black;
        }
        if (Input.GetMouseButtonDown(0) && canSkip)
        {
            if (vidPlayer.isPlaying)
            {
                vidPlayer.Pause();
            }
            PlayerPrefs.SetInt("CanSkipLore", 1);
            SceneLoader.Instance.ToGameplay();
        }
    }
}
