using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour {
    [SerializeField] private AudioMixer _audioMixer;
    public float SFXVolume { get; private set; }
    public float MusicVolume { get; private set; }

    private void Awake() {
        SFXVolume = PlayerPrefs.GetFloat("SFXVol", 0.6f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVol", 0.6f);
    }

    public void SetMusicVolume(float volume) {
        _audioMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVol", volume);
        MusicVolume = volume;
    }

    public void SetSFXVolume(float volume) {
        _audioMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVol", volume);
        SFXVolume = volume;
    }
}