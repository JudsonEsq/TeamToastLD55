using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChange : MonoBehaviour
{
    [SerializeField]
    private AudioClip music1;
    [SerializeField]
    private AudioClip music2;
    [SerializeField]
    private AudioClip music3;

    [SerializeField]
    private ObjectiveManager objMan;

    private AudioSource musicSource;
    private float currentTime = 0;

    private int musicIndex = 0;

    // Update is called once per frame
    void Start()
    {
        musicSource = gameObject.GetComponent<AudioSource>();

        if (objMan != null)
        {
            foreach (Objective o in objMan.GetObjectives())
            {
                o.OnObjectiveCompleted += IncrementMusic;
            }
        }
    }

    void IncrementMusic()
    {
        musicIndex++;
        currentTime = musicSource.time;
        switch (musicIndex)
        {
            case 0:
                musicSource.clip = music1;
                break;
            case 1:
                musicSource.clip = music2;
                break;
            case 2:
                musicSource.clip = music3;
                break;
            default:
                musicSource.clip = music3;
                break;
        }

        musicSource.time = currentTime;
        musicSource.Play();
    }
}
