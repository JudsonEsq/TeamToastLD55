using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
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
