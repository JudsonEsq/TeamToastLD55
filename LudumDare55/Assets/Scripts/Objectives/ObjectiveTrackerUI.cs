using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveTrackerUI : MonoBehaviour
{
    [SerializeField] private GameObject objectiveUIPrefab;
    [SerializeField] private GameObject objectivesUI;
    [SerializeField] private Transform objectivesContent;
    [SerializeField] private ObjectiveManager objectiveManager;

    private Dictionary<Objective, TMP_Text> objectives = new();

    private void Start()
    {
        if (objectiveManager != null)
        {
            objectiveManager.OnAllObjectivesCompleted += OnCompletAllObjectives;
        }

        if (objectivesUI == null) return;
        objectivesUI.SetActive(false);
    }

    private void OnDisable()
    {
        if (objectiveManager != null)
        {
            objectiveManager.OnAllObjectivesCompleted -= OnCompletAllObjectives;
        }
    }

    private void Update()
    {
        // Toggle the UI
        if (Input.GetKeyDown(KeyCode.F))
        {
            OpenUI();
        }
    }

    private void SetUI()
    {
        if (objectivesContent == null) return;
        if (objectiveManager == null) return;
        // if already set the ui, return
        if (objectives.Count != 0) return;

        foreach (var objective in objectiveManager.GetObjectives())
        {
            if (objective == null) return;

            // Create the UI and set the text 
            if (!Instantiate(objectiveUIPrefab, transform.position, Quaternion.identity, objectivesContent).TryGetComponent<TMP_Text>(out var uiText)) continue;

            uiText.text = objective.ObjectiveText;
            // The event subscribers are automtically cleared on objective completion in the objective class,
            // so don't worry about unsubscribing here
            // Make the text update automatically when the objetive text is updated
            objective.OnObjectiveTextUpdated += () => { uiText.text = objective.ObjectiveText; };
            // Strike through the text when the objective is completed
            objective.OnObjectiveCompleted += () =>
            {
                if (uiText.font.fallbackFontAssetTable != null && uiText.font.fallbackFontAssetTable.Count > 0)
                {
                    // I set the strike through font asset as a fallback asset on the font we are using for the objectives
                    uiText.font = uiText.font.fallbackFontAssetTable[0];
                }
                else
                {
                    uiText.fontStyle = FontStyles.Strikethrough;
                }
            };

            // keep track of the objective and the text object
            objectives.Add(objective, uiText);
        }
    }

    private void OpenUI()
    {
        if (objectivesUI == null) return;

        objectivesUI.SetActive(!objectivesUI.activeSelf);

        if (objectivesUI.activeSelf)
        {
            SetUI();
        }
    }

    private void OnCompletAllObjectives()
    {
        // What should happen when all the objectives are completed?

        // Clear current objective cache and destroy corresponding text object?
    }
}
