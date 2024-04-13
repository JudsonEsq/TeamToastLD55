using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas mainMenuSet;
    [SerializeField] private Canvas optionsMenuSet;

    // Triggers all animations for starting the game, then
    // loads main play scene
    public void PlayGame()
    {
        print("Play button triggered");
        // Hide the menu UI
        mainCanvas.enabled = false;

        // Trigger transition animation here, then once complete...

        // Load main play scene.
    }


    public void OpenOptions()
    {
        optionsMenuSet.enabled = true;
        mainMenuSet.enabled = false;
    }

    public void CloseOptions()
    {
        mainMenuSet.enabled = true;
        optionsMenuSet.enabled = false;
    }

}
