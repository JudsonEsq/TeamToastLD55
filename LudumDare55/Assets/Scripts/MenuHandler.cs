using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{

    [SerializeField] private Canvas mainCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
