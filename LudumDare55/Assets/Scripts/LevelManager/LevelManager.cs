using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private bool transitioning = false;

    // Different types of level transitions, or what the level transition is about
    public enum LevelTransition
    {
        None,
        NewGame,
        MainMenu,
        Completed,
    }

    // Any method that needs to know about a level transition can subscribe to this
    // This is called before the Load Scene method itself.
    // This means, You can add a delay to the TransitionLevel when it's called and 
    // have the methods subscribed to this event run before the scene change
    public Action<LevelTransition> OnLevelTransitioned;


    [Header("Level Settings")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset mainMenuLevel;
    [SerializeField] private SceneAsset playableLevel;
    [SerializeField] private SceneAsset endLevel;
#endif

    [Header("Debug")]
    [SerializeField] private string mainMenuLevelName;
    [SerializeField] private string playableLevelName;
    [SerializeField] private string endLevelName;


    private void OnValidate()
    {
#if UNITY_EDITOR
        UpdateSceneNames();
#endif
    }

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

#if UNITY_EDITOR
    // This should only be run while in the editor
    [ContextMenu("Update Scene Names")]
    private void UpdateSceneNames()
    {
        if (Application.isPlaying) return;

        if (mainMenuLevel != null)
        {
            mainMenuLevelName = mainMenuLevel.name;
        }
        if (playableLevel != null)
        {
            playableLevelName = playableLevel.name;
        }
        if (endLevel != null)
        {
            endLevelName = endLevel.name;
        }

        //Debug.Log("Update scene names");
    }
#endif


    public void NewGame(float delay = 0f)
    {
        TransitionLevel(delay, LevelTransition.NewGame);
    }

    public void LoadMainMenu(float delay = 0f)
    {
        TransitionLevel(delay, LevelTransition.MainMenu);
    }

    public void GameComplete(float delay = 0f)
    {
        TransitionLevel(delay, LevelTransition.Completed);
    }


    private void TransitionLevel(float delay = 0f, LevelTransition transitionType = LevelTransition.None)
    {
        if (transitioning) return;

        string sceneName = "";
        switch (transitionType)
        {
            default:
            case LevelTransition.None:
                break;
            case LevelTransition.NewGame:
                if (string.IsNullOrEmpty(playableLevelName)) return;

                sceneName = playableLevelName;
                break;
            case LevelTransition.MainMenu:
                if (string.IsNullOrEmpty(mainMenuLevelName)) return;

                sceneName = mainMenuLevelName;
                break;
            case LevelTransition.Completed:
               if (string.IsNullOrEmpty(endLevelName)) return;

                sceneName = endLevelName;
                break;
        }

        if (string.IsNullOrEmpty(sceneName)) return;

        // This is triggered when a new scene is expected to be loaded
        OnLevelTransitioned?.Invoke(transitionType);

        StartCoroutine(LoadLevel(delay, sceneName));

        transitioning = true;
    }

    private IEnumerator LoadLevel(float delay, string sceneName)
    {
        // Wait for the specified amount of time
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
        transitioning = false;
    }
}