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
#if UNITY_EDITOR
    [SerializeField] private SceneAsset mainMenuLevel;
    [SerializeField] private SceneAsset gameOverLevel;
    [SerializeField] private List<SceneAsset> playableLevels = new();
#endif

    [Header("Debug")]
    [SerializeField] private int currentPlayableLevel = -1;
    [SerializeField] private string mainMenuLevelName;
    [SerializeField] private string gameOverLevelName;
    [SerializeField] private List<string> playableLevelNames = new();


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
        if (gameOverLevel != null)
        {
            gameOverLevelName = gameOverLevel.name;
        }

        if (playableLevels.Count == 0) return;
        playableLevelNames.Clear();
        for (int i = 0; i < playableLevels.Count; i++)
        {
            if (playableLevels[i] == null)
            {
                Debug.LogError("A playable level is null");
                playableLevelNames.Clear();
                return;
            }

            playableLevelNames.Add(playableLevels[i].name);
        }

        //Debug.Log("Update scene names");
    }
#endif

    private void Start()
    {
        // if you start on a playable level, set the correct index
        if (Instance == this)
        {
            var currentScene = SceneManager.GetActiveScene();
            if (playableLevelNames.Count == 0) return;

            var sceneName = playableLevelNames.Find(x => x == currentScene.name);
            if (!string.IsNullOrEmpty(sceneName))
            {
                currentPlayableLevel = playableLevelNames.IndexOf(sceneName);
            }
        }
    }

    private void Update()
    {
        // Solely for testing purposes
        if (Input.GetKeyDown(KeyCode.L))
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
        }
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
                if (playableLevelNames.Count == 0 || playableLevelNames[0] == null) return;

                sceneName = playableLevelNames[0];
                currentPlayableLevel = 0;
                break;
            case LevelTransition.MainMenu:
                if (string.IsNullOrEmpty(mainMenuLevelName)) return;

                sceneName = mainMenuLevelName;
                currentPlayableLevel = -1;
                break;
            case LevelTransition.Failed:
                if (string.IsNullOrEmpty(gameOverLevelName)) return;

                sceneName = gameOverLevelName;
                currentPlayableLevel = -1;
                break;
            case LevelTransition.Passed:
                if (playableLevelNames.Count == 0) return;
                if (currentPlayableLevel == playableLevelNames.Count - 1)
                {
                    // Trigger the event and tell all subscribers all the levels have been completed to prompt what to do next
                    // An example will be to connect this to a UI, which will then give the option to call the NewGame or MainMenu transition
                    OnCompleteAllLevels?.Invoke();
                    return;
                }

                currentPlayableLevel++;
                if (playableLevelNames.Count < currentPlayableLevel || playableLevelNames[currentPlayableLevel] == null) return;

                sceneName = playableLevelNames[currentPlayableLevel];
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
