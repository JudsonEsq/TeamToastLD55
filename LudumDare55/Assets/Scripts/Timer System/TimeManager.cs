using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Not sure if this should be a singleton
    // but for now, it's setup to be placed on all playable scenes/level and you can have different settings for the different levels


    [Tooltip("This is the maximun amount of time allowed on this level before the player fails the level")]
    [SerializeField] private float levelAllotedTime = 10f;
    [Tooltip("This adds a delay to the gameover transition, (OPTIONAL)")]
    [SerializeField] private float gameOverTransitionDelay = 0f;
    [SerializeField] private float currentTime = 0f;
    private bool disableTimer = false;



    private void Start()
    {
        // Convert to seconds
        currentTime = levelAllotedTime * 60f;
        disableTimer = false;

        LevelManager.Instance.OnLevelTransitioned += HandleLevelTransition;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnLevelTransitioned -= HandleLevelTransition;
    }

    private void Update()
    {
        if (disableTimer) return;

        // if the timer is done without the player completing the level, then trigger the gameover transition
        if (currentTime <= 0f)
        {
            LevelManager.Instance.GameOver(gameOverTransitionDelay);
        }

        currentTime -= Time.deltaTime;
    }

    private void HandleLevelTransition(LevelManager.LevelTransition transitionType)
    {
        disableTimer = true;

        // You can also do a bunch of other stuffs based on the type of level transition that was triggered here
    }
}
