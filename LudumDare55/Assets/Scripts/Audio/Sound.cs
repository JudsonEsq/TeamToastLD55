using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    public bool loop;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 0.3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}
