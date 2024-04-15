using System;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    // Template for what an objective is
    public string ObjectiveText { get; private set; }
    public Action OnObjectiveTextUpdated;
    public Action OnObjectiveCompleted;
    public bool Completed { get; private set; }

    protected virtual void Start()
    {
        Completed = false;
    }

    private void Update()
    {
        if (Completed) return;

        if (IsCompleted())
        {
            OnObjectiveCompleted?.Invoke();
            Completed = true;

            // Clear all event subscribers
            OnObjectiveCompleted = null;
            OnObjectiveTextUpdated = null;
        }
    }

    protected abstract bool IsCompleted();
    protected void SetObjectiveText(string text)
    {
        ObjectiveText = text;
        OnObjectiveTextUpdated?.Invoke();
    }
}
