using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorialize : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [Tooltip("A list of keys to be tutorialized")]
    [SerializeField] private KeyCode[] tutorialKeys;
    [Tooltip("The messages to be displayed with each tutorialized key")]
    [SerializeField] private string[] tutorialMessages;

    private int messageIndex = 0;

    private void Awake()
    {
        if(tutorialKeys.Length > 0 && tutorialMessages.Length > 0)
        {
            tutorialText.text = tutorialMessages[0];
        }
    }

    private void IncrementText()
    {
        messageIndex++;
        if(messageIndex >= tutorialMessages.Length - 1)
        {
            Destroy(tutorialText);
            Destroy(gameObject);
            return;
        }
        tutorialText.text = tutorialMessages[messageIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(tutorialKeys[messageIndex]))
        {
            IncrementText();

        }
    }
}
