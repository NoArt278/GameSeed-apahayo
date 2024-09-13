using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    Music, SFX
}

[System.Serializable]
public class Sound
{
    public AudioType type;
    public AudioClip clip;
    public string name;
    [Range(0f,1f)]
    public float volume;
    [Range(.1f,3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;

    public bool loop;
}