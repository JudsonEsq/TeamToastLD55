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
        Failed,
        Passed,
    }

    // Any method that needs to know about a level transition can subscribe to this
    // This is called before the Load Scene method itself.
    // This means, You can add a delay to the TransitionLevel when it's called and 
    // have the methods subscribed to this event run before the scene change
    public Action<LevelTransition> OnLevelTransitioned;

    // This is called when the final level is passed
    public Action OnCompleteAllLevels;

    [Header("Level Settings")]
    [SerializeField] private SceneAsset mainMenuLevel;
    [SerializeField] private SceneAsset gameOverLevel;
    [SerializeField] private List<SceneAsset> playableLevels = new();
    [SerializeField] private int currentPlayableLevel = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        // Solely for testing purposes
        /*if (Input.GetKeyDown(KeyCode.L))
        {
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            LoadMainMenu();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            NewGame();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            TransitionLevel(0f, LevelTransition.Passed);
        }*/
    }

    public void NewGame(float delay = 0f)
    {
        TransitionLevel(delay, LevelTransition.NewGame);
    }

    public void LoadMainMenu(float delay = 0f)
    {
        TransitionLevel(delay, LevelTransition.MainMenu);
    }

    public void GameOver(float delay = 0f)
    {
        TransitionLevel(delay, LevelTransition.Failed);
    }


    public void TransitionLevel(float delay = 0f, LevelTransition transitionType = LevelTransition.None)
    {
        if (transitioning) return;

        string sceneName = "";
        switch (transitionType)
        {
            default:
            case LevelTransition.None:
                currentPlayableLevel = -1;
                break;
            case LevelTransition.NewGame:
                if (playableLevels.Count == 0 || playableLevels[0] == null) return;

                sceneName = playableLevels[0].name;
                currentPlayableLevel = 0;
                break;
            case LevelTransition.MainMenu:
                if (mainMenuLevel == null) return;

                sceneName = mainMenuLevel.name;
                currentPlayableLevel = -1;
                break;
            case LevelTransition.Failed:
                if (gameOverLevel == null) return;

                sceneName = gameOverLevel.name;
                currentPlayableLevel = -1;
                break;
            case LevelTransition.Passed:
                if (playableLevels.Count == 0) return;
                if (currentPlayableLevel == playableLevels.Count - 1)
                {
                    // Trigger the event and tell all subscribers all the levels have been completed to prompt what to do next
                    // An example will be to connect this to a UI, which will then give the option to call the NewGame or MainMenu transition
                    OnCompleteAllLevels?.Invoke();
                    return;
                }

                currentPlayableLevel++;
                if (playableLevels.Count < currentPlayableLevel || playableLevels[currentPlayableLevel] == null) return;

                sceneName = playableLevels[currentPlayableLevel].name;
                break;
        }

        if (string.IsNullOrEmpty(sceneName)) return;

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
