using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class AdvanceScene : MonoBehaviour {
    
	void Update ()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //change scene 
        if (Input.GetKeyDown(KeyCode.Return) || inputDevice.Command.WasPressed)
        {
            LoadNextScene();
        }
	}

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
