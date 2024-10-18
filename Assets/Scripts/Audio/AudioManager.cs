using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonMB<AudioManager> {
    [SerializeField] private List<Sound> _sounds;
    [SerializeField] private int _backgroundChannelAmount = 5;
    [SerializeField] private AudioMixerGroup _backgroundMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    private List<AudioSource> _backgroundChannels;
    private List<AudioSource> _occupiedBackgroundChannels;

    protected override void Awake() {
        base.Awake();

        _backgroundChannels = new List<AudioSource>();
        for (int i = 0; i < _backgroundChannelAmount; i++) {
            AudioSource backgroundSrc = gameObject.AddComponent<AudioSource>();
            backgroundSrc.outputAudioMixerGroup = _backgroundMixerGroup;
            _backgroundChannels.Add(backgroundSrc);
        }

        _occupiedBackgroundChannels = new List<AudioSource>();

        foreach (Sound sound in _sounds) {
            if (sound.Type == AudioType.SFX) {
                AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
                sound.Source = sfxSource;
                sfxSource.outputAudioMixerGroup = _sfxMixerGroup;

                sound.Source.volume = sound.Volume;
                sound.Source.pitch = sound.Pitch;
                sound.Source.loop = sound.Loop;
                sound.Source.clip = sound.Clip;
            }
        }
    }

    public Sound FindSound(string name) {
        return _sounds.Find(s => s.Name == name);
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
        if (sound.Type != AudioType.SFX) {
            Debug.LogWarning("Play One Shot is only for SFX");
            return;
        }

        sound.Source.clip = sound.Clip;
        sound.Source.volume = sound.Volume;
        sound.Source.pitch = sound.Pitch;
        sound.Source.PlayOneShot(sound.Clip);
    }

    public IEnumerator PlayOneShotCor(string name) {
        Sound sound = FindSound(name);
        PlayOneShot(name);
        yield return new WaitForSeconds(sound.Clip.length);
    }

    public void Play(string name, bool overrideExisting = true) {
        Sound sound = _sounds.Find(s => s.Name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        bool isPlaying = sound.Source != null && sound.Source.isPlaying;    
        if (isPlaying && !overrideExisting) return; 

        if (sound.Type == AudioType.Background) {
            AudioSource backgroundSrc;
            if (isPlaying) { // Override existing if already playing
                backgroundSrc = sound.Source;
            } else { // Else, find empty channel
                backgroundSrc = _backgroundChannels.Find(src => !_occupiedBackgroundChannels.Contains(src));
            }

            if (backgroundSrc == null) {
                Debug.LogWarning("No available background channel!");
                return;
            }

            _occupiedBackgroundChannels.Add(backgroundSrc);
            sound.Source = backgroundSrc;
        }

        sound.ActiveTween?.Kill();

        sound.Source.clip = sound.Clip;
        sound.Source.volume = sound.Volume;
        sound.Source.pitch = sound.Pitch;
        sound.Source.Play();
    }

    public void Stop(string name) {
        Sound sound = _sounds.Find(s => s.Name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        sound.Source.Stop();
        if (sound.Type == AudioType.Background) {
            _occupiedBackgroundChannels.Remove(sound.Source);
            sound.Source = null;
        }
    }

    public void StopFadeOut(string name, float fadeTime) {
        Sound sound = _sounds.Find(s => s.Name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        sound.ActiveTween?.Kill();
        sound.Source.volume = sound.Volume;
        sound.ActiveTween = sound.Source.DOFade(0, fadeTime).SetUpdate(true).OnComplete(() => {
            sound.Source.Stop();
            sound.ActiveTween = null;
            if (sound.Type == AudioType.Background) {
                _occupiedBackgroundChannels.Remove(sound.Source);
                sound.Source = null;
            }
        });
    }

    public void StopBGMFadeOut(float fadeTime) {
        foreach (Sound sound in _sounds) {
            if (sound.Type == AudioType.Background && sound.Source != null) {
                sound.ActiveTween?.Kill();
                sound.Source.volume = sound.Volume;
                sound.ActiveTween = sound.Source.DOFade(0, fadeTime).SetUpdate(true).OnComplete(() => {
                    sound.Source.Stop();
                    sound.ActiveTween = null;
                    _occupiedBackgroundChannels.Remove(sound.Source);
                    sound.Source = null;
                });
            }
        }
    }
}