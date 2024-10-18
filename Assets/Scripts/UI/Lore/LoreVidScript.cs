using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class LoreVidScript : MonoBehaviour
{
    [SerializeField] GameObject _skipText;
    [SerializeField] VideoPlayer _vidPlayer;
    private bool _canSkip;

    private void Awake()
    {
        _canSkip = PlayerPrefs.GetInt("CanSkipLore", 0) == 1;
        _skipText.SetActive(_canSkip);
    }

    private void Update()
    {
        if (!_vidPlayer.isPlaying && _vidPlayer.frame == (long) _vidPlayer.frameCount - 1)
        {
            _canSkip = true;
            _skipText.SetActive(true);
            TMP_Text skipT = _skipText.GetComponent<TMP_Text>();
            skipT.text = "Click to continue";
        }
        if (Input.GetMouseButtonDown(0) && _canSkip)
        {
            if (_vidPlayer.isPlaying)
            {
                _vidPlayer.Pause();
            }
            PlayerPrefs.SetInt("CanSkipLore", 1);
            SceneLoader.Instance.ToGameplay();
        }
    }
}
