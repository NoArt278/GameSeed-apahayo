using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }
    [SerializeField] private List<Sound> sounds;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixerGroup sfxGroup;

    private Tween sfxFadeTween;
    private Tween musicFadeTween;

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
                AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
                sound.source = sfxSource;
                sfxSource.outputAudioMixerGroup = sfxGroup;
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
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.PlayOneShot(sound.clip);
    }

    public void StopBGM() {
        musicSource.Stop();
    }

    public void StopBGMFadeOut(float fadeTime) {
        musicFadeTween?.Kill();
        musicFadeTween = musicSource.DOFade(0, fadeTime).SetUpdate(true).OnComplete(() => musicSource.Stop());
    }

    public void Play(string name, bool overrideExisting = true) {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (sound.source.isPlaying && !overrideExisting) return; 

        sfxFadeTween?.Kill();

        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
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

        sfxFadeTween?.Kill();
        sound.source.volume = sound.volume;
        sfxFadeTween = sound.source.DOFade(0, fadeTime).SetUpdate(true).OnComplete(() => sound.source.Stop());
    }
}