using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // This changes based on the level
    private ObjectiveManager currentObjectiveManager;

    // Gives time to any methods that are subscribed to the level transiton event, and allows then to run before the new scene is loaded.
    [SerializeField] private float playableLevelTransitionDelay = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // Subcribe to the leveltransition event
        LevelManager.Instance.OnLevelTransitioned += HandleLevelTransition;
    }

    private void OnDisable()
    {
        // unsubscribe from all events
        LevelManager.Instance.OnLevelTransitioned -= HandleLevelTransition;

        if (currentObjectiveManager != null)
        {
            currentObjectiveManager.OnAllObjectivesCompleted -= UpdateOnCompleteAllObjectives;
        }
    }

    public void SetObjectiveManager(ObjectiveManager objectiveManager)
    {
        // Unsubscribe before setting a new objective manager
        if (currentObjectiveManager != null)
        {
            currentObjectiveManager.OnAllObjectivesCompleted -= UpdateOnCompleteAllObjectives;
        }

        // Set new objective manager
        currentObjectiveManager = objectiveManager;

        // Subscribe to new manager
        if (currentObjectiveManager != null)
        {
            currentObjectiveManager.OnAllObjectivesCompleted += UpdateOnCompleteAllObjectives;
        }
    }

    // What happens when you are on a playable level and complete all objectives
    private void UpdateOnCompleteAllObjectives()
    {
        // Transition to next playable level
        LevelManager.Instance.TransitionLevel(playableLevelTransitionDelay, LevelManager.LevelTransition.Passed);
    }

    // This is called when the Level transition event is triggered
    private void HandleLevelTransition(LevelManager.LevelTransition transitionType)
    {
        if (transitionType != LevelManager.LevelTransition.Passed)
        {
            SetObjectiveManager(null);
        }
    }
}
