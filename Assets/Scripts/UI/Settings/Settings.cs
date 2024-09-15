using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider, sfxSlider;
    [SerializeField] AudioSource musicTest, sfxTest;

    private void Awake()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 50);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 50);
        SetMusicVol(musicSlider.value);
        SetSFXVol(sfxSlider.value);
    }

    public void SetMusicVol(float vol)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(vol) * 20);
        PlayerPrefs.SetFloat("MusicVol", vol);
        musicTest.Play();
    }

    public void SetSFXVol(float vol)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(vol) * 20);
        PlayerPrefs.SetFloat("SFXVol", vol);
        sfxTest.Play();
    }

    public void BackToPrev()
    {
        gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
