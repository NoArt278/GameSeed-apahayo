using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour {
    [SerializeField] private AudioMixer _audioMixer;

    public void SetMusicVolume(float volume) {
        _audioMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVol", volume);
    }

    public void SetSFXVolume(float volume) {
        _audioMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVol", volume);
    }
}