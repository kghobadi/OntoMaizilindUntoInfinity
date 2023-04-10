using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for picking specific scenes from the Scene menu in the pause menu.
/// </summary>
public class SceneMenu : MonoBehaviour
{
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
    
    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }
}
