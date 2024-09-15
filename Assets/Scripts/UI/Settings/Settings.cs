using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider, sfxSlider;
    [SerializeField] AudioSource sfxTest;
    [HideInInspector] public bool isOpen = false;

    private void Awake()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1);
        sfxTest.enabled = false;
        SetMusicVol(musicSlider.value);
        SetSFXVol(sfxSlider.value);
        sfxTest.enabled = true;
    }

    public void OpenSettings()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void SetMusicVol(float vol)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(vol) * 20);
        PlayerPrefs.SetFloat("MusicVol", vol);
    }

    public void SetSFXVol(float vol)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(vol) * 20);
        PlayerPrefs.SetFloat("SFXVol", vol);
        sfxTest.Play();
    }

    public void BackToPrev()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
