using DG.Tweening;
using UnityEngine;

public enum AudioType
{
    Background, SFX
}

[System.Serializable]
public class Sound
{
    public AudioType type;
    public AudioClip clip;
    public string name;
    [Range(0f,1f)]
    public float volume = 0.6f;
    [Range(.1f,3f)]
    public float pitch = 1f;

    [HideInInspector]
    public AudioSource source;

    public bool loop;
    public Tween activeTween;
}