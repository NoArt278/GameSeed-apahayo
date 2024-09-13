using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }
    [SerializeField] private List<Sound> sounds;
    [SerializeField] private AudioSource musicSource;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        foreach (Sound sound in sounds) {
            if (sound.type == AudioType.Music) {
                sound.source = musicSource;
            } else {
                sound.source = gameObject.AddComponent<AudioSource>();
            }
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.clip = sound.clip;
        }
    }

    public void PlayOneShot(string name) {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        sound.source.clip = sound.clip;
        sound.source.PlayOneShot(sound.clip);
    }

    public void Play(string name, bool overrideExisting = true) {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (sound.source.isPlaying && !overrideExisting) return; 

        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;

        sound.source.Stop();
        sound.source.Play();
    }

    public void Stop(string name) {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        sound.source.Stop();
    }

    public void StopFadeOut(string name, float fadeTime) {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        sound.source.DOFade(0, fadeTime);
    }
}