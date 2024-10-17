using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider, sfxSlider;

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 0.6f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 0.6f);
        SetMusicVol(musicSlider.value);
        SetSFXVol(sfxSlider.value);
    }

    public void SetMusicVol(float vol)
    {
        Settings.Instance.Audio.SetMusicVolume(vol);
    }

    public void SetSFXVol(float vol)
    {
        Settings.Instance.Audio.SetSFXVolume(vol);
    }
}
