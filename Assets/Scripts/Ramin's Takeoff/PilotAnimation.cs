using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotAnimation : AnimationHandler
{
    private bool inBarrelRoll;

    public bool IsInBarrelRoll
    {
        get => inBarrelRoll;
        set => inBarrelRoll = value;
    }

    /// <summary>
    /// Called by anim flags. 
    /// </summary>
    public void FinishBarrelRoll()
    {
        IsInBarrelRoll = false;
    }
}
