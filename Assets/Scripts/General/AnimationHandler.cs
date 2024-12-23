﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationHandler : MonoBehaviour
{
    [HideInInspector]
    public Animator characterAnimator;

    //holds all the bools for this animator 
    public string[] animationBools;

    protected virtual void Awake()
    {
        characterAnimator = GetComponent<Animator>();
    }

    //changes animation state (only for boolean paramters)
    public virtual void SetAnimator(string anim)
    {
        if(characterAnimator == null)
            return;

        //turn off all anim bools
        for (int i = 0; i < animationBools.Length; i++)
        {
            if (HasParameter(animationBools[i]))
            {
                characterAnimator.SetBool(animationBools[i], false);
            }
        }

        //set anim bool true
        if (HasParameter(anim))
        {
            characterAnimator.SetBool(anim, true);
        }
    }

    protected bool AnimationIsActive(string anim) { 
        if(characterAnimator == null)
            return false;

        return characterAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim);
    }

    //sets animator speed 
    public float Speed
    {
        get
        {
            if(characterAnimator == null)
                return 0f;

            return characterAnimator.speed;
        }

        set
        {
            if(characterAnimator == null)
                return;

            characterAnimator.speed = value;
        }
    }

    public Animator Animator
    {
        get
        {
            return characterAnimator;
        }
        set
        {
            characterAnimator = value;
        }
    }
    
    /// <summary>
    /// Check if the animator has a parameter of specified name. 
    /// </summary>
    /// <param name="paramName"></param>
    /// <returns></returns>
    public virtual bool HasParameter(string paramName)
    {
        foreach (AnimatorControllerParameter param in Animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}
