using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{

    // How fast the Options menu scrolls up and down when opened and closed, in percent
    [SerializeField] private float optionsSpeed = 1f;

    // Various UI elements or groups of UI elements for activating and deactivating
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Image background;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void OpenOptions()
    {
        optionsMenuSet.enabled = true;

        StartCoroutine(OptionsArrive());

        mainMenuSet.enabled = false;
    }

    // Coroutine for sliding the options up from the bottom of the screen
    IEnumerator OptionsArrive()
    {
        float progress = 0f;

        while (progress < 1f)
        {
            print(progress);
            progress += 0.01f * optionsSpeed;
            optionsMenuSet.transform.Translate(Vector3.up * progress);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void CloseOptions()
    {
        mainMenuSet.enabled = true;
        optionsMenuSet.enabled = false;
    }

}
