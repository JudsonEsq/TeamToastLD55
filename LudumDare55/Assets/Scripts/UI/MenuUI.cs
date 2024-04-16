using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OpenMainMenu()
    {
        if (LevelManager.Instance == null) return;

        LevelManager.Instance.LoadMainMenu();
    }
    public void StartGame()
    {
        if (LevelManager.Instance == null) return;

        LevelManager.Instance.NewGame();
    }

    public void OpenCredits()
    {
        if (LevelManager.Instance == null) return;

        LevelManager.Instance.GameComplete();
    }
}
