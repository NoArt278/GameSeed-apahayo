using System.Collections.Generic;
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

    public void Play(string name) {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

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
}