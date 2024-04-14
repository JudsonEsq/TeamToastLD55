using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTargets : Objective
{
    [Tooltip("How many targets the player must break to succeed this objective")]
    [SerializeField] private int totalTargets = 1;

    private int targetsBroken = 0;

    public void TargetBroken()
    {
        targetsBroken++;
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
