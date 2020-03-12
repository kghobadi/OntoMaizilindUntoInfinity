using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class AdvanceScene : MonoBehaviour {

    public float timeToRestart = 5f;
    public float restartTimer;

	void Update ()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //next scene 
        if (Input.GetKeyDown(KeyCode.Return) || inputDevice.RightBumper.WasPressed)
        {
            LoadNextScene();
        }

        //previous scene 
        if (Input.GetKeyDown(KeyCode.CapsLock) || inputDevice.LeftBumper.WasPressed)
        {
            LoadPreviousScene();
        }

        //restart game
        if (Input.GetKey(KeyCode.Delete) || (inputDevice.Command))
        {
            restartTimer += Time.deltaTime;

            if(restartTimer > timeToRestart)
            {
                Restart();
            }
        }
        else
        {
            restartTimer = 0;
        }
	}

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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
