using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorialize : MonoBehaviour
{
    [SerializeField] private TextMeshPro tutorialText;
    [Tooltip("A list of keys to be tutorialized")]
    [SerializeField] private KeyCode[] tutorialKeys;
    [Tooltip("The messages to be displayed with each tutorialized key")]
    [SerializeField] private string[] tutorialMessages;

    private void Awake()
    {
        if(tutorialKeys.Length > 0 && tutorialMessages.Length > 0)
        {
            tutorialText.text = tutorialMessages[0];
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
