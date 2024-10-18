using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _sfxSlider;

    private void Start()
    {
        _musicSlider.value = Settings.Instance.Audio.MusicVolume;
        _sfxSlider.value = Settings.Instance.Audio.SFXVolume;
        SetMusicVol(_musicSlider.value);
        SetSFXVol(_sfxSlider.value);
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
