using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private List<Objective> objectives = new();
    public List<Objective> GetObjectives() { return objectives; }

    public Action OnAllObjectivesCompleted;
    private bool allCompleted = false;

    private float nextLoop;
    private float updateFrequency = 0.2f;


    private void Start()
    {
        allCompleted = false;

        // Update the current objective manager on the GameManager, since there are different instances of this on the different playable level
        GameManager.Instance.SetObjectiveManager(this);
    }

    private void Update()
    {
        if (allCompleted) return;

        if (Time.time > nextLoop)
        {
            if (CheckObjectiveStatus())
            {
                OnAllObjectivesCompleted?.Invoke();
                allCompleted = true;
            }

            nextLoop = Time.time + updateFrequency;
        }
    }

    private bool CheckObjectiveStatus()
    {
        if (objectives.Count == 0) return false;

        foreach (var objective in objectives)
        {
            if (objective == null)
            {
                Debug.LogError("There is a null objective, this should not happen", gameObject);
                return false;
            }

            if (!objective.Completed)
            {
                return false;
            }
        }


        return true;
    }
}
