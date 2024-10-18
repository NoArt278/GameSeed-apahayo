using DG.Tweening;
using UnityEngine;

public enum AudioType
{
    Background, SFX
}

[System.Serializable]
public class Sound
{
    public AudioType Type;
    public AudioClip Clip;
    public string Name;
    [Range(0f,1f)] public float Volume = 0.6f;
    [Range(.1f,3f)] public float Pitch = 1f;
    [HideInInspector] public AudioSource Source;
    public bool Loop;
    public Tween ActiveTween;
}