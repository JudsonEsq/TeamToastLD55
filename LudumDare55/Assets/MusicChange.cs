using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChange : MonoBehaviour
{
    [SerializeField]
    private float volume = 0.3f;

    [SerializeField]
    private AudioSource music1;
    [SerializeField]
    private AudioSource music2;
    [SerializeField]
    private AudioSource music3;

    [SerializeField]
    private ObjectiveManager objMan;

    private AudioSource musicSource;

    private float currentTime = 0;

    private int musicIndex = 0;

    // Update is called once per frame
    void Start()
    {
        musicSource = music1;
        music2.volume = 0;
        music3.volume = 0;

        musicSource.Play();
        music2.Play();
        music3.Play();

        if (objMan != null)
        {
            foreach (Objective o in objMan.GetObjectives())
            {
                o.OnObjectiveCompleted += IncrementMusic;
            }
        }
    }

    IEnumerator FadeIn(AudioSource nextSong)
    {
        while(nextSong.volume < volume)
        {
            nextSong.volume += .003f;
            yield return null;
        }
    }

    IEnumerator FadeOut(AudioSource lastSong)
    {
        while (lastSong.volume > 0)
        {
            lastSong.volume -= .003f;
            yield return null;
        }
    }

    void IncrementMusic()
    {
        musicIndex++;
        currentTime = musicSource.time;
        switch (musicIndex)
        {
            case 0:
                StartCoroutine(FadeOut(musicSource));
                StartCoroutine(FadeIn(music1));
                break;
            case 1:
                StartCoroutine(FadeOut(musicSource));
                StartCoroutine(FadeIn(music2));
                break;
            default:
                StartCoroutine(FadeOut(music1));
                StartCoroutine(FadeOut(music2));
                StartCoroutine(FadeIn(music3));
                break;
        }

        musicSource.time = currentTime;
        musicSource.Play();
    }
}
