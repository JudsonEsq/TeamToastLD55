using System;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    // Template for what an objective is

    public Action OnObjectiveCompleted;
    public bool Completed {get; private set;}

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
        }
    }

    protected abstract bool IsCompleted();
}
