using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTargets : Objective
{
    [Tooltip("How many targets the player must break to succeed this objective")]
    [SerializeField] private int totalTargets = 1;

    [Tooltip("Target's name to be broken")]
    [SerializeField] private string targetName = "";

    private int targetsBroken = 0;

    protected override void Start()
    {
        base.Start();

        var text = $"Break {targetName} : {targetsBroken} / {totalTargets}";
        SetObjectiveText(text);
    }

    public void TargetBroken()
    {
        targetsBroken++;
        
        // Update the objective text
        var text = $"Break {targetName} : {targetsBroken} / {totalTargets}";
        SetObjectiveText(text);
    }

    protected override bool IsCompleted()
    {

        if (targetsBroken >= totalTargets)
        {
            return true;
        }

        return false;
    }
}
