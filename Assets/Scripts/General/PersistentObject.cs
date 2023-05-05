using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simply sets any game object to Dont Destroy on Load. 
/// </summary>
public class PersistentObject : MonoBehaviour
{
    private LoadSceneAsync _loadSceneAsync;
    private AdvanceScene _advanceScene;
    public int scenesLoaded = 0;
    public int scenesMax;
    
    void Awake()
    {
        _advanceScene = FindObjectOfType<AdvanceScene>();
        _loadSceneAsync = FindObjectOfType<LoadSceneAsync>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        //events
        if (_advanceScene)
        {
            _advanceScene.onSceneLoad.AddListener(IncrementSceneCounter);
        }
        else if(_loadSceneAsync)
        {
            _loadSceneAsync.onSceneLoad.AddListener(IncrementSceneCounter); 
        }
    }

    private void OnDisable()
    {
        //events
        if (_advanceScene)
        {
            _advanceScene.onSceneLoad.RemoveListener(IncrementSceneCounter);
        }
        else if(_loadSceneAsync)
        {
            _loadSceneAsync.onSceneLoad.RemoveListener(IncrementSceneCounter); 
        }
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

