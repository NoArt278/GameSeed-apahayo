using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonMB<AudioManager> {
    [SerializeField] private List<Sound> sounds;
    [SerializeField] private int backgroundChannelAmount = 5;
    [SerializeField] private AudioMixerGroup backgroundMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private List<AudioSource> backgroundChannels;
    private List<AudioSource> occupiedBackgroundChannels;

    protected override void Awake() {
        base.Awake();

        backgroundChannels = new List<AudioSource>();
        for (int i = 0; i < backgroundChannelAmount; i++) {
            AudioSource backgroundSrc = gameObject.AddComponent<AudioSource>();
            backgroundSrc.outputAudioMixerGroup = backgroundMixerGroup;
            backgroundChannels.Add(backgroundSrc);
        }

        occupiedBackgroundChannels = new List<AudioSource>();

        foreach (Sound sound in sounds) {
            if (sound.type == AudioType.SFX) {
                AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
                sound.source = sfxSource;
                sfxSource.outputAudioMixerGroup = sfxMixerGroup;

                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
                sound.source.clip = sound.clip;
            }
        }
    }

    public Sound FindSound(string name) {
        return sounds.Find(s => s.name == name);
    }

    public void PlayOneShot(string name) {
        Sound sound = FindSound(name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        PlayOneShot(sound);
    }

    public void PlayOneShot(Sound sound) {
        if (sound.type != AudioType.SFX) {
            Debug.LogWarning("Play One Shot is only for SFX");
            return;
        }

        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.PlayOneShot(sound.clip);
    }

    public IEnumerator PlayOneShotCor(string name) {
        Sound sound = FindSound(name);
        PlayOneShot(name);
        yield return new WaitForSeconds(sound.clip.length);
    }

    public void Play(string name, bool overrideExisting = true) {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (sound.source != null && sound.source.isPlaying && !overrideExisting) return; 

        if (sound.type == AudioType.Background) {
            AudioSource backgroundSrc = backgroundChannels.Find(s => !occupiedBackgroundChannels.Contains(s));
            if (backgroundSrc == null) {
                Debug.LogWarning("No available background channel!");
                return;
            }

            occupiedBackgroundChannels.Add(backgroundSrc);
            sound.source = backgroundSrc;
        }

        sound.activeTween?.Kill();

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
        if (sound.type == AudioType.Background) {
            occupiedBackgroundChannels.Remove(sound.source);
            sound.source = null;
        }
    }

    public void StopFadeOut(string name, float fadeTime) {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        sound.activeTween?.Kill();
        sound.source.volume = sound.volume;
        sound.activeTween = sound.source.DOFade(0, fadeTime).SetUpdate(true).OnComplete(() => {
            sound.source.Stop();
            sound.activeTween = null;
            if (sound.type == AudioType.Background) {
                occupiedBackgroundChannels.Remove(sound.source);
                sound.source = null;
            }
        });
    }

    public void StopBGMFadeOut(float fadeTime) {
        foreach (Sound sound in sounds) {
            if (sound.type == AudioType.Background && sound.source != null) {
                sound.activeTween?.Kill();
                sound.source.volume = sound.volume;
                sound.activeTween = sound.source.DOFade(0, fadeTime).SetUpdate(true).OnComplete(() => {
                    sound.source.Stop();
                    sound.activeTween = null;
                    occupiedBackgroundChannels.Remove(sound.source);
                    sound.source = null;
                });
            }
        }
    }
}