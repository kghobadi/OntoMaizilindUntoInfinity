using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simply sets any game object to Dont Destroy on Load. 
/// </summary>
public class PersistentObject : MonoBehaviour
{
    private AdvanceScene _advanceScene;
    public int scenesLoaded = 0;
    public int scenesMax;
    
    void Awake()
    {
        _advanceScene = FindObjectOfType<AdvanceScene>();
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        //events
        _advanceScene.onSceneLoad.AddListener(IncrementSceneCounter);
    }

    private void OnDisable()
    {
        //events
        _advanceScene.onSceneLoad.RemoveListener(IncrementSceneCounter);
    }

    void IncrementSceneCounter()
    {
        scenesLoaded++;

        if (scenesLoaded >= scenesMax)
        {
            Destroy(gameObject);
        }
    }
}

